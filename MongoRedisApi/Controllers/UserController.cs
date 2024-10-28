using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoRedisApi.Models;
using MongoRedisApi.Services;

namespace MongoRedisApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly RedisCacheService _redisCacheService;
        private readonly ILogger<UserController> _logger;
        public UserController(UserService userService,RedisCacheService redisCacheService, ILogger<UserController> logger)
        {
            _userService = userService;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Get all users");

            var cachedUsers = await _redisCacheService.GetCacheAsync<List<User>>("allUsers");

            if ( cachedUsers!=null && cachedUsers.Count!=0)
            {       _logger.LogInformation($"{cachedUsers.Count} cache users found");
                    return Ok(cachedUsers);
            }
            _logger.LogInformation($"No cache users found. Users getting from db");
            var users= await _userService.GetAllAsync();
            await _redisCacheService.SetCacheAsync<List<User>>("allUsers", users);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id )
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody]User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
    }
}
