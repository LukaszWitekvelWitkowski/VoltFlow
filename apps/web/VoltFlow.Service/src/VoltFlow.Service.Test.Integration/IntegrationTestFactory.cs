using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using System.Linq; // Dodaj to using, jeśli go brakuje
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using VoltFlow.Service.Infrastructure.Data;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    // TUTAJ BYŁ BRAK - Deklarujemy pola klasy:
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Inicjalizacja połączenia
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        // KROK 1: Budowa schematu bazy (migracje/tabele)
        var optionsBuilder = new DbContextOptionsBuilder<VoltFlowDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString());

        using (var context = new VoltFlowDbContext(optionsBuilder.Options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        // KROK 2: Konfiguracja Respawnera (teraz widzi tabele)
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        // Używamy zadeklarowanego pola _dbConnection
        await _respawner.ResetAsync(_dbConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VoltFlowDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<VoltFlowDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public new async Task DisposeAsync()
    {
        // Używamy zadeklarowanego pola _dbConnection
        if (_dbConnection != null) await _dbConnection.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}