using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth;
using Galaxy.Auth.Presentation.Helpers;
using Galaxy.Teams;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using User = Galaxy.Auth.User;

namespace Galaxy.Auth.Presentation.Services
{
    public class UserService: User.UserBase
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public UserService(IUserService userService, ILoginService loginService)
        {
            _userService = userService;
            _loginService = loginService;
        }

        public override async Task<UserReply> GetUser(UserRequest user, ServerCallContext context)
        {
            var dbUser = await _userService.VerifyUserExistsAsync(user.Username);
            return new UserReply
            {
                Id = dbUser?.Id ?? 0,
                Name = dbUser?.Name
            }; 
        }

        public override async Task<ActionReplay> Activate(TokenModel request, ServerCallContext context)
        {
            var response = await _userService.ActivateAsync(request.Token);
            return response.ToActionReplay();
        }

        public override async Task<ActionReplay> Update(UpdateRequest request, ServerCallContext context)
        {
            var response = await _userService.UpdateAsync(request.Username, request.Name, request.Phone);
            return response.ToActionReplay();
        }

        public override async Task<TokenModel> Login(LoginRequest request, ServerCallContext context)
        {
            var user = await _loginService.LoginAsync(request.Email, request.Password);

            return new TokenModel()
            {
                Token = _loginService.GenerateJwtToken(user)
            };
        }

        public override async Task<ActionReplay> Register(RegisterRequest request, ServerCallContext context)
        {
            var response = await _userService.RegisterAsync(new RegisterModel
            {
                Email =  request.Email,
                Name = request.Name,
                Password = request.Password
            });
            return response.ToActionReplay();
        }

        public override async Task<ActionReplay> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            var response = await _userService.ChangePasswordAsync(request.Username, request.NewPassword, request.OldPassword);
            return response.ToActionReplay();
        }
    }
}