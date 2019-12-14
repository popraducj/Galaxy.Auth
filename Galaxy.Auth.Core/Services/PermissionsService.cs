using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;

namespace Galaxy.Auth.Core.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IUserPermissionRepository _repository;

        public PermissionsService(IUserPermissionRepository repository)
        {
            _repository = repository;
        }
        public async Task AddPermissions(List<Permission> permissions)
        {
            await _repository.AddAsync(permissions);
        }

        public async Task RemovePermissions(List<Permission> permissions)
        {
            await _repository.RemoveAsync(permissions);
        }

        public Task<List<Permission>> GetByUserIdAsync(int userId)
        {
           return _repository.GetByUserIdAsync(userId);
        }
    }
}