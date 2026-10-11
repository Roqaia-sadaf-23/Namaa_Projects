namespace Namaa.Application.Abstractions.Persistence;

public interface IQuotationRepository
{
    Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
