using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace Namaa.Tests.Common;

public sealed class TestLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    public IReadOnlyCollection<LogEvent> Events => _events.ToArray();

    public void Emit(LogEvent logEvent) => _events.Enqueue(logEvent);

    public void Clear()
    {
        while (_events.TryDequeue(out _))
        {
        }
    }
}
