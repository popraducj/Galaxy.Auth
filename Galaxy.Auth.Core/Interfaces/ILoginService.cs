using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface ILoginService
    {
        Task<User> Login(string email, string password);
        string GenerateJwtToken(IdentityUser<int> user);
    }
}