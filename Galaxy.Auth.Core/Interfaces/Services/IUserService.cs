using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityError>> RegisterAsync(RegisterModel model);
        Task<IEnumerable<IdentityError>> ActivateAsync(string token);
        Task<IEnumerable<IdentityError>> UpdateAsync(string username, string name, string phone);
        Task<IEnumerable<IdentityError>> ChangePasswordAsync(string username, string newPassword, string oldPassword);
        Task<User> VerifyUserExistsAsync(string username);
    }
}