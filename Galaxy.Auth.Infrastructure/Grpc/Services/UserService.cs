using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Grpc;
using Grpc.Core;

namespace Galaxy.Auth.Infrastructure.Grpc.Services
{
    public class UserService: User.UserBase
    {
        private readonly IUserService _userService;

        public UserService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<UserReply> VerifyUser(UserRequest user, ServerCallContext context)
        {
            var dbUser = await _userService.VerifyUserExists(user.Username);
            return new UserReply
            {
                Success = dbUser != null,
                Id = dbUser?.Id ?? 0
            }; 
        }
    }
}