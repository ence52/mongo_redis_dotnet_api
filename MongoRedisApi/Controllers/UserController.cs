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
        private readonly ILogger<UserController> _logger;
        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id )
        {
            var user = await _userService.GetUserByIdAsync(id);

            return user != null ? Ok(user) : NotFound() ;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody]User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Delete request recieved with an empty ID.");
                return BadRequest("ID is required");
            }

            var result = await _userService.DeleteUserAsync(id);

            if (result)
            {
                _logger.LogInformation("User {id} deleted succesfully",id);
                return Ok(new {Message="User deleted succesfully."});
            }
            else
            {
                _logger.LogWarning("User deletion failed or user not found: {id}",id);
                return NotFound(new { Message = "User not found or could not be deleted." });
            }

            
        }
    }
}
