using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByIdAsync(string userId);
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<IdentityResult> DeleteAsync(TUser user);
        Task<string> GenerateChangeEmailTokenAsync(TUser user, string newEmail);
        Task<string> GeneratePasswordResetTokenAsync(TUser user);
        Task<IdentityResult> ChangeEmailAsync(TUser user, string newEmail, string token);
        Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);
        Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
    }
}
