using System.Threading.Tasks;
using Galaxy.Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Galaxy.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<bool> Register(UserRegisterViewModel model)
        {
            await Task.CompletedTask;
            return true;
        }
        
        [HttpPost("update")]
        public async Task<bool> Update(UserUpdateViewModel model)
        {
            await Task.CompletedTask;
            return true;
        }

        [HttpPost("changePassword")]
        public async Task<bool> ChangePassword(ChangePasswordViewModel model)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}