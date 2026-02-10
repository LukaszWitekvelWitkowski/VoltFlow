using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Test.Integration.IntegrationTest
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>, IAsyncLifetime
    {
        protected readonly HttpClient Client;
        protected readonly IntegrationTestFactory Factory;
        protected readonly VoltFlowDbContext DbContext;
        protected readonly IServiceScope _scope;

        protected BaseIntegrationTest(IntegrationTestFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<VoltFlowDbContext>();
        }

        public async Task InitializeAsync()
        {
            await Factory.ResetDatabaseAsync();

            // TO JEST KLUCZOWE:
            var cache = _scope.ServiceProvider.GetService<IMemoryCache>() as MemoryCache;
            if (cache != null)
            {
                cache.Compact(1.0); // Wymuszenie usunięcia 100% wpisów
            }
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }
    }
}
