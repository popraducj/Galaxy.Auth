using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;

namespace Galaxy.Auth.Core.Interfaces.Services
{
    public interface IPermissionsService
    {
        Task<ActionResponse> AddPermissions(List<Permission> permissions);
        Task<ActionResponse> RemovePermissions(List<Permission> permissions);

        Task<List<Permission>> GetByUserIdAsync(int userId);
    }
}