using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Namaa.Tests.Common;

public sealed class NamaaWebApplicationFactory(
    string connectionString,
    string signingKey,
    string storageRoot,
    TestLogSink logSink) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.Sources.Clear();
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Jwt:Issuer"] = "Namaa.AcceptanceTests",
                ["Jwt:Audience"] = "Namaa.AcceptanceTests.Client",
                ["Jwt:SigningKey"] = signingKey,
                ["Jwt:AccessTokenMinutes"] = "15",
                ["Jwt:RefreshTokenDays"] = "1",
                ["FileStorage:Provider"] = "Local",
                ["FileStorage:LocalRoot"] = storageRoot,
                ["FileStorage:MaxFileSizeBytes"] = "1024",
                ["FileStorage:BlockedExtensions:0"] = ".exe",
                ["FileStorage:BlockedExtensions:1"] = ".ps1",
                ["FileStorage:BlockedExtensions:2"] = ".html",
                ["FileStorage:BlockedContentTypes:0"] = "application/x-msdownload",
                ["FileStorage:BlockedContentTypes:1"] = "text/html",
                ["AllowedHosts"] = "*"
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseSerilog((context, _, configuration) => configuration
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
            .WriteTo.Sink(logSink));

        return base.CreateHost(builder);
    }
}
