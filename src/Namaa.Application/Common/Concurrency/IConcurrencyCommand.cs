namespace Namaa.Application.Common.Concurrency;

public interface IConcurrencyCommand
{
    string RowVersion { get; }
}
