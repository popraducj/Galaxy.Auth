using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;

namespace Galaxy.Auth.Core.Interfaces.Services
{
    public interface IPermissionsService
    {
        Task AddPermissions(List<Permission> permissions);
        Task RemovePermissions(List<Permission> permissions);

        Task<List<Permission>> GetByUserIdAsync(int userId);
    }
}