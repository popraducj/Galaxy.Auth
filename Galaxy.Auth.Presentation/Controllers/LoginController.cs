using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Galaxy.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;

        public LoginController(ILogger<LoginController> logger, ILoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<LoginResultViewModel> GetJwtToken(LoginViewModel model)
        {
            var user = await _loginService.Login(model.Email, model.Password);

            return new LoginResultViewModel
            {
                Token = _loginService.GenerateJwtToken(user)
            };
        }
    }
}