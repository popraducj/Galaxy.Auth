using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Enums;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth;
using Galaxy.Auth.Presentation.Helpers;
using Galaxy.Teams;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Galaxy.Auth.Presentation.Services
{
    public class PermissionsService : Permissions.PermissionsBase
    {
        private readonly IPermissionsService _permissionsService;
        private readonly ILogger<PermissionsService> _logger;

        public PermissionsService(IPermissionsService permissionsService, ILogger<PermissionsService> logger)
        {
            _permissionsService = permissionsService;
            _logger = logger;
        }
        
        public override async Task<ActionReplay> AddPermission(IAsyncStreamReader<PermissionRequest> requestStream, ServerCallContext context)
        { 
            var response = await _permissionsService.AddPermissions(await GetPermissions(requestStream));
            return response.ToActionReplay();
        }
        
        public override async Task<ActionReplay> RemovePermission(IAsyncStreamReader<PermissionRequest> requestStream, ServerCallContext context)
        {
            var response = await _permissionsService.RemovePermissions(await GetPermissions(requestStream));
            return response.ToActionReplay();
        }

        public override async  Task GetPermissions(UserPermissionRequest user, IServerStreamWriter<UserPermissionReplay> responseStream, ServerCallContext context)
        {
            var permissions = await _permissionsService.GetByUserIdAsync(user.Id);
            foreach (var permission in permissions)
            {
                await responseStream.WriteAsync(new UserPermissionReplay
                {
                    Permission = (int)permission.UserPermission
                });
            }
        }

        private static async Task<List<Permission>> GetPermissions(IAsyncStreamReader<PermissionRequest> requestStream)
        {
            var permissions = new List<Permission>();
            await foreach (var permission in requestStream.ReadAllAsync())
            {
                permissions.Add(new Permission
                {
                    UserId =  permission.UserId,
                    UserPermission = (UserPermission)permission.Permission
                });
            }

            return permissions;
        }
    }
}