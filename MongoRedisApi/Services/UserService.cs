using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRedisApi.Models;
using MongoRedisApi.Services.Abstract;
using MongoRedisApi.Utilities;

namespace MongoRedisApi.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly RedisCacheService _redisCacheService;
        private readonly ILogger<UserService> _logger;
        public UserService(IMongoClient mongoClient, RedisCacheService redisCacheService, ILogger<UserService> logger)
        {
            var database = mongoClient.GetDatabase("TestDB");
            _users = database.GetCollection<User>("Users");
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task CreateUserAsync(User user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            await  _users.InsertOneAsync(user);
            _logger.LogInformation("User {0} created and added to db.", user.Id);
            await _redisCacheService.SetCacheWithPrefixAsync(user);
            _logger.LogInformation("User {0} cached.", user.Id);

        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            if (!MongoDbHelper.isValidObjectId(id))
            {
                _logger.LogWarning("Invalid userId: {id}",id);
                return false;
            }
            try
            {
                var deleteResult = await _users.DeleteOneAsync(u => u.Id == id);

                if (deleteResult.DeletedCount > 0)
                {
                    _logger.LogInformation("User Deleted: {id}", id);
                    await _redisCacheService.DeleteKeySetMemberAsync("User",id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("User not found: {id}", id);
                    return false;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occured while deleting the user: {id}", id);
                return false;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            _logger.LogInformation("Get all users");

            var cachedUsers = await _redisCacheService.GetAllCacheAsync<User>("allUsers");

            if (cachedUsers != null && cachedUsers.Count != 0)
            {
                _logger.LogInformation($"{cachedUsers.Count} cached users found");
                return cachedUsers;

            }
            _logger.LogInformation($"No cache users found. Users getting from db");
            var users = await _users.Find(_ => true).ToListAsync();
            await _redisCacheService.SetListCacheAsync("allUsers", users);
            return users;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var dbUser= await _users.Find(u=>u.Id==id).FirstOrDefaultAsync();

            return dbUser;

        }
    }
}
