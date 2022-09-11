using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null)
                return;

            var serializedResponse = JsonConvert.SerializeObject(response);
            await distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive
            });
        }

        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            var cachedResponse = await distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cachedResponse) ? "" : cachedResponse;
        }

        public async Task RemoveCache(string cacheKey)
        {
            await distributedCache.RemoveAsync(cacheKey);
        }

    }
}
