using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Authentication.Dtos;
using Namaa.Domain.Entities;
using Namaa.Domain.Enums;
using Namaa.Infrastructure.Persistence;

namespace Namaa.Tests.Common;

public sealed partial class NamaaApiFixture : IAsyncLifetime
{
    private const string TestConnectionVariable = "NAMAA_TEST_SQLSERVER_CONNECTION_STRING";
    private readonly string _databaseName = $"NamaaAcceptance_{Guid.NewGuid():N}";
    private readonly Dictionary<string, string?> _originalEnvironment = new(StringComparer.Ordinal);
    private string _adminConnectionString = string.Empty;

    public NamaaWebApplicationFactory Factory { get; private set; } = null!;
    public TestLogSink LogSink { get; } = new();
    public string DatabaseConnectionString { get; private set; } = string.Empty;
    public string StorageRoot { get; } = Path.Combine(
        Path.GetTempPath(),
        "NamaaAcceptanceStorage",
        Guid.NewGuid().ToString("N"));
    public string SigningKey { get; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    public string ActiveEmail { get; } = $"active-{Guid.NewGuid():N}@example.test";
    public string InactiveEmail { get; } = $"inactive-{Guid.NewGuid():N}@example.test";
    public string Password { get; } = $"T!{Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))}";
    public long ActiveUserId { get; private set; }
    public long CustomerId { get; private set; }

    public async Task InitializeAsync()
    {
        var configured = Environment.GetEnvironmentVariable(TestConnectionVariable);
        var serverConnection = string.IsNullOrWhiteSpace(configured)
            ? @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;Connect Timeout=30"
            : configured;

        var baseBuilder = new SqlConnectionStringBuilder(serverConnection)
        {
            InitialCatalog = "master"
        };
        _adminConnectionString = baseBuilder.ConnectionString;

        await using (var connection = new SqlConnection(_adminConnectionString))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE [{_databaseName}]";
            await command.ExecuteNonQueryAsync();
        }

        baseBuilder.InitialCatalog = _databaseName;
        DatabaseConnectionString = baseBuilder.ConnectionString;
        await ApplySchemaAsync();

        SetHostEnvironment("ConnectionStrings__DefaultConnection", DatabaseConnectionString);
        SetHostEnvironment("Jwt__Issuer", "Namaa.AcceptanceTests");
        SetHostEnvironment("Jwt__Audience", "Namaa.AcceptanceTests.Client");
        SetHostEnvironment("Jwt__SigningKey", SigningKey);
        SetHostEnvironment("Jwt__AccessTokenMinutes", "15");
        SetHostEnvironment("Jwt__RefreshTokenDays", "1");
        SetHostEnvironment("FileStorage__Provider", "Local");
        SetHostEnvironment("FileStorage__LocalRoot", StorageRoot);
        SetHostEnvironment("FileStorage__MaxFileSizeBytes", "1024");
        SetHostEnvironment("FileStorage__BlockedExtensions__0", ".exe");
        SetHostEnvironment("FileStorage__BlockedExtensions__1", ".ps1");
        SetHostEnvironment("FileStorage__BlockedExtensions__2", ".html");
        SetHostEnvironment("FileStorage__BlockedContentTypes__0", "application/x-msdownload");
        SetHostEnvironment("FileStorage__BlockedContentTypes__1", "text/html");

        Factory = new NamaaWebApplicationFactory(
            DatabaseConnectionString,
            SigningKey,
            StorageRoot,
            LogSink);
        _ = Factory.Services;
        await SeedIdentityAndCustomerAsync();
    }

    public async Task DisposeAsync()
    {
        Factory?.Dispose();
        SqlConnection.ClearAllPools();

        foreach (var pair in _originalEnvironment)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }

        if (string.IsNullOrWhiteSpace(_adminConnectionString))
        {
            return;
        }

        await using var connection = new SqlConnection(_adminConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}];";
        await command.ExecuteNonQueryAsync();

        if (Directory.Exists(StorageRoot))
        {
            Directory.Delete(StorageRoot, true);
        }
    }

    public HttpClient CreateClient() => Factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost")
    });

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var token = await LoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return client;
    }

    public async Task<AuthTokenDto> LoginAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = ActiveEmail,
            Password
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthTokenDto>())!;
    }

    public async Task<T> WithDbContextAsync<T>(Func<NamaaDbContext, Task<T>> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<NamaaDbContext>();
        return await action(db);
    }

    public async Task WithDbContextAsync(Func<NamaaDbContext, Task> action)
    {
        await WithDbContextAsync(async db =>
        {
            await action(db);
            return true;
        });
    }

    public async Task<Project> SeedProjectAsync(string? name = null)
    {
        return await WithDbContextAsync(async db =>
        {
            var project = Project.Create(
                CustomerId,
                NewCode("PRJ"),
                name ?? "Acceptance project",
                1250m,
                ActiveUserId);
            db.Projects.Add(project);
            await db.SaveChangesAsync();
            return project;
        });
    }

    public async Task<Account> SeedAccountAsync(string? name = null)
    {
        return await WithDbContextAsync(async db =>
        {
            var account = Account.Create(NewCode("AC"), name ?? "Acceptance account", "Asset");
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
            return account;
        });
    }

    public async Task<ProjectTask> SeedProjectTaskAsync(long projectId)
    {
        return await WithDbContextAsync(async db =>
        {
            var task = ProjectTask.Create(projectId, "Scoped task", ActiveUserId, TaskPriority.Medium);
            db.ProjectTasks.Add(task);
            await db.SaveChangesAsync();
            return task;
        });
    }

    public async Task<ProjectFile> SeedProjectFileAsync(long projectId, string storageKey)
    {
        return await WithDbContextAsync(async db =>
        {
            var file = ProjectFile.Create(
                projectId,
                ActiveUserId,
                "acceptance.pdf",
                storageKey,
                128,
                "application/pdf");
            db.ProjectFiles.Add(file);
            await db.SaveChangesAsync();
            return file;
        });
    }

    public async Task<(PaymentVoucher Voucher, PurchaseInvoice Invoice)> SeedVoucherScenarioAsync()
    {
        return await WithDbContextAsync(async db =>
        {
            var supplier = Supplier.Create(NewCode("SUP"), "Acceptance supplier");
            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();

            var invoice = PurchaseInvoice.Create(
                supplier.Id,
                NewCode("PINV"),
                DateTime.UtcNow.Date,
                ActiveUserId);
            var voucher = PaymentVoucher.Create(
                NewCode("PV"),
                DateTime.UtcNow.Date,
                "BankTransfer",
                100m,
                ActiveUserId,
                supplier.Id);
            db.PurchaseInvoices.Add(invoice);
            db.PaymentVouchers.Add(voucher);
            await db.SaveChangesAsync();
            return (voucher, invoice);
        });
    }

    public static string NewCode(string prefix) => $"{prefix}-{Guid.NewGuid():N}"[..Math.Min(30, prefix.Length + 9)];

    private async Task ApplySchemaAsync()
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Database", "NamaaDatabaseSchema.sql");
        var script = await File.ReadAllTextAsync(schemaPath, Encoding.UTF8);
        var batches = GoBatchRegex().Split(script);

        await using var connection = new SqlConnection(DatabaseConnectionString);
        await connection.OpenAsync();
        foreach (var batch in batches)
        {
            var sql = batch.Trim();
            if (sql.Length == 0 || sql.StartsWith("USE ", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task SeedIdentityAndCustomerAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<NamaaDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var role = Role.Create("AcceptanceUser", "Test-only authenticated policy role");
        db.Roles.Add(role);
        await db.SaveChangesAsync();

        var active = User.Create(role.Id, "Active acceptance user", ActiveEmail, null, hasher.Hash(Password));
        var inactive = User.Create(role.Id, "Inactive acceptance user", InactiveEmail, null, hasher.Hash(Password));
        inactive.Deactivate();
        var customer = Customer.Create(NewCode("CUS"), "Acceptance customer");
        db.Users.AddRange(active, inactive);
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        ActiveUserId = active.Id;
        CustomerId = customer.Id;
    }

    private void SetHostEnvironment(string name, string value)
    {
        _originalEnvironment[name] = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
    }

    [GeneratedRegex(@"^\s*GO\s*(?:--.*)?$", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex GoBatchRegex();
}
