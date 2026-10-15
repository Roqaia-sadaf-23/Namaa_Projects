namespace Namaa.Application.Abstractions.Persistence;

public interface IConcurrencyTokenContext
{
    void UseExpectedRowVersion(byte[] rowVersion);

    void ClearExpectedRowVersion();
}
