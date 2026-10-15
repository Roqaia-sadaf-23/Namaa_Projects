namespace Namaa.Application.Abstractions.Time;

public interface IUtcClock
{
    DateTime UtcNow { get; }
}
