using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IQuotationRepository
{
    Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(string quotationNumber, CancellationToken cancellationToken = default);
    Task<Quotation?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Quotation?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quotation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Quotation quotation, CancellationToken cancellationToken = default);
}
