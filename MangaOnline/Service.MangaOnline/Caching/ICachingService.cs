namespace Service.MangaOnline.Caching;

public interface ICachingService
{
    Task SetAsync(string key, string value);
    Task<string> GetAsync(string key);

    Task SetRecordAsync<T>(string key,
        T data,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null);
    Task<T?> GetRecordAsync<T>(string key);
}