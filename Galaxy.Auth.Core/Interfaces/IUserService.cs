using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityError>> Register(RegisterModel model);
        Task<IEnumerable<IdentityError>> Activate(string token);
        Task<IEnumerable<IdentityError>> Update(string id, string name, string phone);
        Task<IEnumerable<IdentityError>> ChangePassword(string id, string newPassword, string oldPassword);
    }
}