using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class QuotationRepository(NamaaDbContext dbContext) : IQuotationRepository
{
    public Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Quotations
            .AsNoTracking()
            .AnyAsync(quotation => quotation.Id == id, cancellationToken);
}
