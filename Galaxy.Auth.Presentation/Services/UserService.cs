using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using User = Galaxy.Auth.Grpc.User;

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

        public override async Task<UserActionReplay> Activate(TokenModel request, ServerCallContext context)
        {
            var errors = (await _userService.ActivateAsync(request.Token)).ToList();
            return GetResponse(errors);
        }

        public override async Task<UserActionReplay> Update(UpdateRequest request, ServerCallContext context)
        {
            var errors = await _userService.UpdateAsync(request.Username, request.Name, request.Phone);
            return GetResponse(errors);
        }

        public override async Task<TokenModel> Login(LoginRequest request, ServerCallContext context)
        {
            var user = await _loginService.LoginAsync(request.Email, request.Password);

            return new TokenModel()
            {
                Token = _loginService.GenerateJwtToken(user)
            };
        }

        public override async Task<UserActionReplay> Register(RegisterRequest request, ServerCallContext context)
        {
            var errors = await _userService.RegisterAsync(new RegisterModel
            {
                Email =  request.Email,
                Name = request.Name,
                Password = request.Password
            });
            return GetResponse(errors);
        }

        public override async Task<UserActionReplay> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            var errors = await _userService.ChangePasswordAsync(request.Username, request.NewPassword, request.OldPassword);
            return GetResponse(errors);
        }

        private UserActionReplay GetResponse(IEnumerable<IdentityError> errors)
        {
            var identityErrors = errors.ToList();
            var response = new UserActionReplay {Success = !identityErrors.Any()};
            
            identityErrors.ForEach(err =>
            {
                response.Errors.Add(new ActionError
                {
                    Code = err.Code,
                    Description = err.Description
                });
            });
            return response;
        }
    }
}