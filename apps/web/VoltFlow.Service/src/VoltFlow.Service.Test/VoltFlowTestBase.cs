using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Test
{
    public abstract class VoltFlowTestBase : IDisposable
    {
        protected readonly VoltFlowDbContext _context;
        protected readonly IConfiguration _configuration;

        protected VoltFlowTestBase()
        {
            // 1. Setup bazy danych InMemory z unikalną nazwą dla każdego testu
            var options = new DbContextOptionsBuilder<VoltFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VoltFlowDbContext(options);

            // 2. Setup domyślnej konfiguracji (możesz ją nadpisać w testach)
            var defaultSettings = new Dictionary<string, string>
            {
                {"RepositorySettings:MaxCacheThreshold", "100"},
                {"RepositorySettings:IsCacheEnabled", "true"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(defaultSettings)
                .Build();
        }

        /// <summary>
        /// Pomocnicza metoda do szybkiego "zasiania" bazy danych przed testem
        /// </summary>
        protected async Task SeedDataAsync<T>(IEnumerable<T> entities) where T : class
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public virtual void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
