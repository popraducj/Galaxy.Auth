using System.Collections.Generic;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Enums;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Grpc.Core;

namespace Galaxy.Auth.Grpc.Services
{
    public class PermissionsRpcService : Permissions.PermissionsBase
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsRpcService(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }
        
        public override async Task<PermissionReply> AddPermission(IAsyncStreamReader<PermissionRequest> requestStream, ServerCallContext context)
        { 
            while (await requestStream.MoveNext())
            {
                var point = requestStream.Current;
                var a = point.Permission;
            }
            return  new PermissionReply();
        }
        
        public override async Task<PermissionReply> RemovePermission(IAsyncStreamReader<PermissionRequest> requestStream, ServerCallContext context)
        {
            try
            {
                await _permissionsService.RemovePermissions(await GetPermissions(requestStream));
                return new PermissionReply{Success = true};
            }
            catch
            {
                return new PermissionReply{Success = false};
            }
        }

        private async Task<List<Permission>> GetPermissions(IAsyncStreamReader<PermissionRequest> requestStream)
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