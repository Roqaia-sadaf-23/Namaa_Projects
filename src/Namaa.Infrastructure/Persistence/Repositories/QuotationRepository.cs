using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class QuotationRepository(NamaaDbContext dbContext) : IQuotationRepository
{
    public Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Quotations
            .AsNoTracking()
            .AnyAsync(quotation => quotation.Id == id, cancellationToken);

    public Task<bool> ExistsByNumberAsync(string quotationNumber, CancellationToken cancellationToken = default) =>
        dbContext.Quotations.AsNoTracking().AnyAsync(x => x.QuotationNumber == quotationNumber, cancellationToken);

    public Task<Quotation?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Quotations.AsNoTracking().Include(x => x.Items).Include(x => x.PaymentTerms).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Quotation?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Quotations.Include(x => x.Items).Include(x => x.PaymentTerms).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Quotation>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Quotations.AsNoTracking().Include(x => x.Items).Include(x => x.PaymentTerms).OrderByDescending(x => x.IssueDate).ThenBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(Quotation quotation, CancellationToken cancellationToken = default) =>
        await dbContext.Quotations.AddAsync(quotation, cancellationToken);
}
