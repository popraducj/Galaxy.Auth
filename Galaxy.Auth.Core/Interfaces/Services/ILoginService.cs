using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Core.Interfaces.Services
{
    public interface ILoginService
    {
        Task<User> LoginAsync(string email, string password);
        string GenerateJwtToken(IdentityUser<int> user);
    }
}