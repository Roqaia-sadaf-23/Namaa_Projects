using System.Globalization;
using Microsoft.Extensions.Configuration;
using Namaa.Api;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Namaa.Tests.Logging;

public sealed class TechnicalLoggingTests
{
    [Fact]
    public void ConfigurationInitializesWithoutAddingSecretConfigurationValues()
    {
        const string connectionString = "Server=example;Password=database-secret";
        const string signingKey = "jwt-signing-secret";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Jwt:SigningKey"] = signingKey,
                ["Serilog:MinimumLevel:Default"] = "Information"
            })
            .Build();
        var sink = new RecordingSink();
        using var logger = TechnicalLogging.Configure(
                new LoggerConfiguration(),
                configuration,
                "Namaa.Api",
                "Test")
            .WriteTo.Sink(sink)
            .CreateLogger();

        logger.Information("Application startup completed");

        var logEvent = Assert.Single(sink.Events);
        var rendered = logEvent.RenderMessage(CultureInfo.InvariantCulture);
        Assert.DoesNotContain(connectionString, rendered, StringComparison.Ordinal);
        Assert.DoesNotContain(signingKey, rendered, StringComparison.Ordinal);
        Assert.Equal("Namaa.Api", ((ScalarValue)logEvent.Properties["ApplicationName"]).Value);
        Assert.Equal("Test", ((ScalarValue)logEvent.Properties["EnvironmentName"]).Value);
    }

    private sealed class RecordingSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }
}
