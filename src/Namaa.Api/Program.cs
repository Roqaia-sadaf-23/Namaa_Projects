using System.Globalization;
using Microsoft.OpenApi.Models;
using Namaa.Api;
using Namaa.Api.Authentication;
using Namaa.Api.Authorization;
using Namaa.Api.Middleware;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application;
using Namaa.Infrastructure;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

WebApplicationBuilder? builder = null;

try
{
    Log.Information("Starting Namaa API");

    builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, _, configuration) =>
        TechnicalLogging.Configure(
            configuration,
            context.Configuration,
            context.HostingEnvironment.ApplicationName,
            context.HostingEnvironment.EnvironmentName));

    // The API composes the layers; use cases and persistence details stay outside HTTP endpoints.
    builder.Services.AddControllers(options =>
        options.Conventions.Add(new BusinessModuleAuthorizationConvention()));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a JWT bearer token."
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = Array.Empty<string>()
        });
    });
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddNamaaAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, _, exception) =>
            exception is not null || httpContext.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode >= 400
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
    });

    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Namaa API terminated unexpectedly during startup or execution");

    if (builder?.Environment.IsEnvironment("Testing") == true)
    {
        throw;
    }

    Environment.ExitCode = 1;
}
finally
{
    Log.Information("Namaa API stopped");
    await Log.CloseAndFlushAsync();
}

public partial class Program;
