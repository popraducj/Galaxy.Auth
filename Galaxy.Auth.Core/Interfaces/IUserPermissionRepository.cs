using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IUserPermissionRepository
    {
        Task<ActionResponse> AddAsync(IEnumerable<Permission> permissions);
        Task<ActionResponse> RemoveAsync(IEnumerable<Permission> permissions);
        Task<List<Permission>> GetByUserIdAsync(int userId);
    }
}