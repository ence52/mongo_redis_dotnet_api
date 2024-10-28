namespace MongoRedisApi.Services.Abstract
{
    public interface ICacheService
    {
        public Task SetCacheAsync<T>(string key, T value);
        public Task<T> GetCacheAsync<T>(string key);
    }
}
