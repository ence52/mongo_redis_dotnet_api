using MongoDB.Bson;
using MongoDB.Driver;
using MongoRedisApi.Models;
using MongoRedisApi.Services.Abstract;

namespace MongoRedisApi.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("TestDB");
            _users = database.GetCollection<User>("Users");
        }

        public async Task CreateUserAsync(User user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            await  _users.InsertOneAsync(user);

        }

        public Task DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(_=>true).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var dbUser= await _users.Find(u=>u.Id==id).FirstOrDefaultAsync();
            return dbUser;

        }
    }
}
