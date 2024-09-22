using Bulky.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CacheService:ICacheService         
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var serializedValue = await _distributedCache.GetStringAsync(key);
            if (serializedValue == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(serializedValue);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
