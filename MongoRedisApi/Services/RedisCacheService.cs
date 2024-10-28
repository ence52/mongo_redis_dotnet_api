using MongoRedisApi.Services.Abstract;
using StackExchange.Redis;
using System.Text.Json;

namespace MongoRedisApi.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDB;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redisDB = redis.GetDatabase();
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            
            var json=  await _redisDB.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json);

        }

        public async Task SetCacheAsync<T>(string key, T value)
        {
            var json= JsonSerializer.Serialize(value);
           await _redisDB.StringSetAsync(key, json);
        }
    }
}
