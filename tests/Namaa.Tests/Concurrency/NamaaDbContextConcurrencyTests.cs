using Microsoft.EntityFrameworkCore;
using Namaa.Domain.Entities;
using Namaa.Infrastructure.Persistence;

namespace Namaa.Tests.Concurrency;

public sealed class NamaaDbContextConcurrencyTests
{
    [Fact]
    public async Task SaveChangesAsyncRejectsMultipleModifiedRowVersionRootsForOneToken()
    {
        var options = new DbContextOptionsBuilder<NamaaDbContext>()
            .UseSqlServer("Server=(local);Database=NotUsed;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        await using var context = new NamaaDbContext(options);
        var first = Account.Create("1000", "Cash", "Asset");
        var second = Account.Create("2000", "Payables", "Liability");
        SetId(first, 1);
        SetId(second, 2);
        context.AttachRange(first, second);
        first.Update("Cash on Hand", "Asset", true);
        second.Update("Trade Payables", "Liability", true);
        context.UseExpectedRowVersion(new byte[8]);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => context.SaveChangesAsync());

        Assert.Contains("exactly one modified RowVersion root", exception.Message);
    }

    private static void SetId(Account account, long id)
    {
        var idProperty = typeof(Account).GetProperty(nameof(Account.Id));
        Assert.NotNull(idProperty);
        idProperty.SetValue(account, id);
    }
}
