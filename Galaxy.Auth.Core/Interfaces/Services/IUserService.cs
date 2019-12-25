using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<ActionResponse> RegisterAsync(RegisterModel model);
        Task<ActionResponse> ActivateAsync(string token);
        Task<ActionResponse> UpdateAsync(string username, string name, string phone);
        Task<ActionResponse> ChangePasswordAsync(string username, string newPassword, string oldPassword);
        Task<User> VerifyUserExistsAsync(string username);
    }
}