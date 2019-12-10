using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Galaxy.Auth.Core.UserManagerExtensions
{
    public class ExtendedSignInManager<TUser> : SignInManager<TUser>, ISignInManager<TUser> where TUser : class
    {
        public ExtendedSignInManager(UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes, IUserConfirmation<TUser> userConfirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, userConfirmation)
        {
            
        }
    }
}
