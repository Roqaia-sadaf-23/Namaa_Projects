using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Accounts.Dtos;
using Namaa.Application.JournalEntries.Dtos;
using Namaa.Application.PaymentVouchers.Dtos;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Finance;

[Collection(AcceptanceTestCollection.Name)]
public sealed class FinanceAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task UpdateAccount_WithStaleRowVersion_Returns409AndPreservesAcceptedState()
    {
        var account = await fixture.SeedAccountAsync("Original account");
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var original = await client.GetFromJsonAsync<AccountDto>($"/api/accounts/{account.Id}");

        using var acceptedResponse = await client.PutAsJsonAsync($"/api/accounts/{account.Id}", new
        {
            AccountName = "Accepted account update",
            original!.AccountType,
            original.IsPostingAllowed,
            original.Description,
            original.ParentAccountId,
            original.RowVersion
        });
        Assert.Equal(HttpStatusCode.OK, acceptedResponse.StatusCode);
        var accepted = (await acceptedResponse.Content.ReadFromJsonAsync<AccountDto>())!;
        Assert.NotEqual(original.RowVersion, accepted.RowVersion);

        using var staleResponse = await client.PutAsJsonAsync($"/api/accounts/{account.Id}", new
        {
            AccountName = "Rejected stale account update",
            original.AccountType,
            original.IsPostingAllowed,
            original.Description,
            original.ParentAccountId,
            original.RowVersion
        });
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        var current = await client.GetFromJsonAsync<AccountDto>($"/api/accounts/{account.Id}");
        Assert.Equal("Accepted account update", current!.AccountName);
        Assert.Equal(accepted.RowVersion, current.RowVersion);
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync(log =>
            log.EntityId == account.Id && log.Action == "Account.Updated"));
        Assert.Equal(1, auditCount);
    }

    [Fact]
    public async Task JournalPosting_BalancedPostsAndAudits_UnbalancedIsRejectedAtomically()
    {
        var account = await fixture.SeedAccountAsync();
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var balanced = await CreateJournalAsync(client);

        var debit = await AddLineAsync(client, balanced.Id, account.Id, 50m, 0m, balanced.RowVersion);
        var credit = await AddLineAsync(client, balanced.Id, account.Id, 0m, 50m, debit.ParentRowVersion);

        using (var anonymous = fixture.CreateClient())
        using (var unauthorizedPost = await anonymous.PostAsJsonAsync(
                   $"/api/journal-entries/{balanced.Id}/post",
                   new { PostedAt = DateTime.UtcNow, RowVersion = credit.ParentRowVersion }))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedPost.StatusCode);
        }

        using var post = await client.PostAsJsonAsync($"/api/journal-entries/{balanced.Id}/post", new
        {
            PostedAt = DateTime.UtcNow,
            RowVersion = credit.ParentRowVersion
        });
        Assert.Equal(HttpStatusCode.OK, post.StatusCode);
        var posted = (await post.Content.ReadFromJsonAsync<JournalEntryDto>())!;
        Assert.Equal("Posted", posted.Status);

        var unbalanced = await CreateJournalAsync(client);
        var onlyDebit = await AddLineAsync(client, unbalanced.Id, account.Id, 25m, 0m, unbalanced.RowVersion);
        using var rejected = await client.PostAsJsonAsync($"/api/journal-entries/{unbalanced.Id}/post", new
        {
            PostedAt = DateTime.UtcNow,
            RowVersion = onlyDebit.ParentRowVersion
        });
        Assert.Equal(HttpStatusCode.Conflict, rejected.StatusCode);

        await fixture.WithDbContextAsync(async db =>
        {
            var rejectedEntity = await db.JournalEntries.AsNoTracking().SingleAsync(entry => entry.Id == unbalanced.Id);
            Assert.Equal("Draft", rejectedEntity.Status);
            Assert.True(await db.AuditLogs.AnyAsync(log =>
                log.EntityId == balanced.Id && log.Action == "JournalEntry.Posted"));
            Assert.False(await db.AuditLogs.AnyAsync(log =>
                log.EntityId == unbalanced.Id && log.Action == "JournalEntry.Posted"));
        });
    }

    [Fact]
    public async Task AddPaymentVoucherAllocation_OverAmount_Returns409WithoutPartialPersistenceOrAudit()
    {
        var scenario = await fixture.SeedVoucherScenarioAsync();
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var voucher = await client.GetFromJsonAsync<PaymentVoucherDto>(
            $"/api/payment-vouchers/{scenario.Voucher.Id}");

        using var response = await client.PostAsJsonAsync(
            $"/api/payment-vouchers/{scenario.Voucher.Id}/allocations",
            new
            {
                PurchaseInvoiceId = scenario.Invoice.Id,
                AllocatedAmount = 100.01m,
                voucher!.RowVersion
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.PaymentVoucherAllocations.AnyAsync(allocation =>
                allocation.PaymentVoucherId == scenario.Voucher.Id));
            Assert.False(await db.AuditLogs.AnyAsync(log =>
                log.Action == "PaymentVoucherAllocation.Added" &&
                log.EntityId != null &&
                log.CreatedAt >= scenario.Voucher.CreatedAt));
        });
    }

    private static async Task<JournalEntryDto> CreateJournalAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync("/api/journal-entries", new
        {
            EntryNumber = NamaaApiFixture.NewCode("JE"),
            EntryDate = DateTime.UtcNow.Date,
            Description = "Acceptance journal",
            SourceType = (string?)null,
            SourceId = (long?)null
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JournalEntryDto>())!;
    }

    private static async Task<JournalEntryLineDto> AddLineAsync(
        HttpClient client,
        long journalId,
        long accountId,
        decimal debit,
        decimal credit,
        string rowVersion)
    {
        using var response = await client.PostAsJsonAsync($"/api/journal-entries/{journalId}/lines", new
        {
            AccountId = accountId,
            Debit = debit,
            Credit = credit,
            BranchId = (long?)null,
            CostCenterId = (long?)null,
            ProjectId = (long?)null,
            Description = (string?)null,
            RowVersion = rowVersion
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JournalEntryLineDto>())!;
    }
}
