using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<string> GeneratePasswordResetTokenAsync(TUser user);
        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);
        Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
    }
}
