using MongoRedisApi.Models;

namespace MongoRedisApi.Services.Abstract
{
    public interface IUserService
    {
        public  Task<List<User>> GetAllAsync();
        public Task<User> GetUserByIdAsync(string id);
        public Task CreateUserAsync(User user);
        public Task<bool> DeleteUserAsync(string id);
    }
}
