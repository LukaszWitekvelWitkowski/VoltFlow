using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Generic;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public abstract class CacheRepository<T, D, TEntity> : BaseRepository
        where T : class, ICacheData<D>, new() // new() pozwala na 'new T()'
        where D : class
        where TEntity : class
    {
        protected static T? _cache;
        protected bool _isCacheEnabled;
        protected readonly int _maxCacheThreshold;
        protected static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public CacheRepository(VoltFlowDbContext context, IConfiguration configuration): base(context, configuration)
        {
            var maxCacheThresholdSection = configuration.GetSection("RepositorySettings:MaxCacheThreshold");
            _maxCacheThreshold = int.TryParse(maxCacheThresholdSection.Value, out var threshold) ? threshold : 5000;

            var isCacheEnabledSection = configuration.GetSection("RepositorySettings:IsCacheEnabled");
            _isCacheEnabled = bool.TryParse(isCacheEnabledSection.Value, out var enabled) ? enabled : true;
        }

        public abstract D MapToDto(TEntity e);

        protected async Task<IEnumerable<D>> FetchFromDbInternal()
        {
            var data = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            return data.Select(MapToDto);
        }

        protected async Task<T> GetOrUpdateCacheAsync()
        {
            if (!_isCacheEnabled) return null!;

            if (_cache != null) return _cache;

            await _lock.WaitAsync();
            try
            {
                if (_cache == null)
                {
                    var count = await _context.Set<TEntity>().CountAsync();
                    if (count > _maxCacheThreshold)
                    {
                        _isCacheEnabled = false;
                        return null!;
                    }
                    _cache = new T();
                    _cache.insert(await FetchFromDbInternal());
                }
                return _cache;
            }
            finally
            {
                _lock.Release();
            }
        }


        public static void ResetStaticCache()
        {
            _lock.Wait();
            try
            {
                _cache = null;
            }
            finally
            {
                _lock.Release();
            }
        }





    }
}
