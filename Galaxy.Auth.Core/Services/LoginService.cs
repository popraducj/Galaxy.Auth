using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Core.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Galaxy.Auth.Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> _logger;
        private readonly ISignInManager<User> _signInManager;
        private readonly IUserManager<User> _userManager;
        private readonly AppSettings _appSettings;

        public LoginService(ILogger<LoginService> logger, ISignInManager<User> signInManager, IUserManager<User> userManager,
            IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException();
            
            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            if (result.Succeeded) return user;
            
            _logger.LogWarning($"Authentication failed for username {user.UserName}");
            throw new UnauthorizedAccessException();
        }
        
        public string GenerateJwtToken(IdentityUser<int> user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.UserName)
            };

            var token = new JwtSecurityToken();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(30);
            
            try
            {


                token = new JwtSecurityToken(_appSettings.Jwt.Issuer,
                    _appSettings.Jwt.Audience,
                    claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            }
            catch (Exception ex)
            {
                var b = ex;
            }
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}