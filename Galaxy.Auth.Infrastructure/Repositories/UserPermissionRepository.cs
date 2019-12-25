using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Galaxy.Auth.Infrastructure.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly AuthDbContext _dbContext;

        public UserPermissionRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResponse> AddAsync(IEnumerable<Permission> permissions)
        {
            try
            {
                await _dbContext.Permissions.AddRangeAsync(permissions);
                await _dbContext.SaveChangesAsync();
                return new ActionResponse();
            }
            catch
            {
                return ActionResponse.FailedToAdd();
            }
        }

        public async Task<ActionResponse> RemoveAsync(IEnumerable<Permission> permissions)
        {
            try
            {
                _dbContext.Permissions.RemoveRange(permissions);
                await _dbContext.SaveChangesAsync();
                return new ActionResponse();
            }
            catch
            {
                return ActionResponse.FailedToAdd();
            }
        }

        public Task<List<Permission>> GetByUserIdAsync(int userId)
        {
            return _dbContext.Permissions.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}