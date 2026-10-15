using Namaa.Application.Abstractions.Time;

namespace Namaa.Infrastructure.Authentication;

public sealed class SystemUtcClock : IUtcClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
