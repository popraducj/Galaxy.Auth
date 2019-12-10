using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Presentation.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Galaxy.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            var user = new RegisterModel
            {
                Email = model.Email,
                Name = model.Name,
                Password = model.Password
            };

            var errors = await _userService.Register(user);
            return GenerateResult(errors);
        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate(string activationToken)
        {
            var errors = await _userService.Activate(activationToken);
            return GenerateResult(errors);
        }
        
        [HttpPost("update")]
        public async Task<IActionResult> Update(UserUpdateViewModel model)
        {
            var errors = await _userService.Update(model.Id, model.Name, model.Phone);
            return GenerateResult(errors);
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var errors = await _userService.ChangePassword(model.Id, model.NewPassword, model.OldPassword);
            return GenerateResult(errors);
        }

        private IActionResult GenerateResult(IEnumerable<IdentityError> errors)
        {
            if (errors == null) return Ok();
            
            return new ObjectResult(new OperationFailedViewModel
            {
                Errors = errors.ToList()
            })
            {
                StatusCode = (int) HttpStatusCode.BadRequest
            };
        }
    }
}