using MongoRedisApi.Abstract;
using MongoRedisApi.Services.Abstract;
using StackExchange.Redis;
using System.Diagnostics;
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
        public async Task<List<T>> GetAllCacheAsync<T>(string key) where T:IBaseModel
        {
            if(!await _redisDB.KeyExistsAsync(key))
            {
                return new List<T> { };
            }

            var dbIds = await _redisDB.SetMembersAsync(key);
            var tasks = dbIds.Select(id=>_redisDB.StringGetAsync($"{typeof(T).Name.ToLower()}-{id}"));
            var results = await Task.WhenAll(tasks);
            var dbItems = results
                .Where(result => !string.IsNullOrEmpty(result))
                .Select(result => JsonSerializer.Deserialize<T>(result))
                .ToList();
            return dbItems;
        }

        public async Task SetCacheAsync<T>(string key, T value)
        {
            var json= JsonSerializer.Serialize(value);
           await _redisDB.StringSetAsync(key, json);
        }
        public async Task SetListCacheAsync<T>(string key, List<T> items) where T: IBaseModel
        {
            if (items==null ||items.Count==0)
            {
                return;
            }


            var itemIds = items.Select(item=>(RedisValue)item.Id).ToArray();
            await _redisDB.SetAddAsync(key, itemIds);

            string typeOfT = typeof(T).Name.ToLower();
            var tasks = items.Select(item =>
            {
                var newKey = $"{typeOfT}-{item.Id}";
                return SetCacheAsync(newKey, item);

            });

            await Task.WhenAll(tasks);

        }

        public async Task SetCacheWithPrefixAsync<T> (T value) where T: IBaseModel
        {
            string tClassName = value.GetType().Name;
            string tAllKey = "all" +tClassName+"s";
            bool keyExists = await _redisDB.KeyExistsAsync(tAllKey);
            if (keyExists)
            {
                await _redisDB.SetAddAsync(tAllKey, value.Id);
                await SetCacheAsync($"{tClassName.ToLower()}-{value.Id}", value);
            }
            else
            {
                await _redisDB.SetAddAsync(tAllKey,value.Id);
                await SetCacheAsync($"{tClassName.ToLower()}-{value.Id}", value);
            }
        }

        public async Task DeleteKeyAsync(string key)
        {
            await _redisDB.KeyDeleteAsync(key);
        }

        public async Task DeleteKeySetMemberAsync(string objectType,string id)
        {
            string all = $"all{objectType}s";
            string idWithPrefix = objectType.ToLower()+"-" + id;
            await _redisDB.SetRemoveAsync(all,id);
            await DeleteKeyAsync(idWithPrefix);
        }
    }
}
