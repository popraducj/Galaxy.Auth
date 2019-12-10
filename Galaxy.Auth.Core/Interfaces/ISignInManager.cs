using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface ISignInManager<in TUser>
    {
        Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);

        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent,
            bool lockoutOnFailure);
    }
}
