using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Service.MangaOnline.Caching;

public class CachingService : ICachingService
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _options;

    public CachingService(IDistributedCache cache)
    {
        _cache = cache;
        _options = new DistributedCacheEntryOptions
        {
            // AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            // SlidingExpiration = TimeSpan.FromSeconds(1200)
        };
    }

    public async Task SetAsync(string key, string value)
    {
        await _cache.SetStringAsync(key, value);
    }

    public async Task<string> GetAsync(string key)
    {
        return await _cache.GetStringAsync(key);
    }

    public async Task SetRecordAsync<T>(string key,
        T data,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        options.SlidingExpiration = slidingExpiration;
        var jsonData = JsonConvert.SerializeObject(data);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task<T?> GetRecordAsync<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);
        if (jsonData is null)
        {
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}