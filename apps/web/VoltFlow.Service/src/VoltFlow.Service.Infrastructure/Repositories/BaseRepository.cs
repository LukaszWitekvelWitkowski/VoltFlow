using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class BaseRepository 
    {
        protected bool _isCacheEnabled;
        protected readonly int _maxCacheThreshold;
        protected readonly VoltFlowDbContext _context;
        protected static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public BaseRepository(VoltFlowDbContext context, IConfiguration configuration)
        {
            _context = context;

            var maxCacheThresholdSection = configuration.GetSection("RepositorySettings:MaxCacheThreshold");
            _maxCacheThreshold = int.TryParse(maxCacheThresholdSection.Value, out var threshold) ? threshold : 5000;

            var isCacheEnabledSection = configuration.GetSection("RepositorySettings:IsCacheEnabled");
            _isCacheEnabled = bool.TryParse(isCacheEnabledSection.Value, out var enabled) ? enabled : true;
        }
    }
}
