namespace MongoRedisApi.Services.Abstract
{
    public interface ICacheService
    {
        public Task SetCacheAsync<T>(string key, T value);
        public Task<T> GetCacheAsync<T>(string key);
        public Task DeleteKeyAsync(string key);
        public Task DeleteKeySetMemberAsync(string setName,string key);
    }
}
