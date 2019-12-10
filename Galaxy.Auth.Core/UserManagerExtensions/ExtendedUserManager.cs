using System;
using System.Collections.Generic;
using Galaxy.Auth.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Galaxy.Auth.Core.UserManagerExtensions
{
    public class ExtendedUserManager<TUser>: UserManager<TUser>, IUserManager<TUser> where TUser : class
    {
        public ExtendedUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
        }
    }
}
