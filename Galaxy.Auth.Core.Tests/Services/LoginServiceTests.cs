using System;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Core.Models.Settings;
using Galaxy.Auth.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Galaxy.Auth.Core.Tests.Services
{
  /*  private readonly ILogger<LoginService> _logger;
    private readonly ISignInManager<User> _signInManager;
    private readonly IUserManager<User> _userManager;
    private readonly AppSettings _appSettings;
*/
    public class LoginServiceTests
    {
        private readonly Mock<ILogger<LoginService>> _logger;
        private readonly Mock<ISignInManager<User>> _signInManager;
        private readonly Mock<IUserManager<User>> _userManager;
        private readonly Mock<IOptions<AppSettings>> _appSettings;
        private const string _email = "test@test.com";
        private const string _password = "Test123!";

        public LoginServiceTests()
        {
            _logger = new Mock<ILogger<LoginService>>();
            _signInManager = new Mock<ISignInManager<User>>();
            _userManager = new Mock<IUserManager<User>>();
            _appSettings = new Mock<IOptions<AppSettings>>();
        }
        
        [Fact]
        public void LoginAsync_FailedToFindEmail_ThrowsUnauthorized()
        {
           // arrange
           _userManager.Setup(x => x.FindByEmailAsync(_email)).ReturnsAsync((User) null);
           var service = GetService();

           // act && assert
           Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await service.LoginAsync(_email, null));
        }
        
        [Fact]
        public void LoginAsync_AuthenticationFailed_ThrowsUnauthorized()
        {
            var user = new User
            {
                Email = _email,
                UserName = Guid.NewGuid().ToString()
            };
            // arrange
            _userManager.Setup(x => x.FindByEmailAsync(_email)).ReturnsAsync(user);
            _signInManager.Setup(x => x.PasswordSignInAsync(user.UserName, _password, false, false))
                .ReturnsAsync(SignInResult.Failed);
            var service = GetService();

            // act && assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await service.LoginAsync(_email, _password));
        }
        
        [Fact]
        public async Task LoginAsync_Succeed()
        {
            // arrange
            var user = new User
            {
                Email = _email,
                UserName = Guid.NewGuid().ToString()
            };
            _userManager.Setup(x => x.FindByEmailAsync(_email)).ReturnsAsync(user);
            _signInManager.Setup(x => x.PasswordSignInAsync(user.UserName, _password, false, false))
                .ReturnsAsync(SignInResult.Success);
            var service = GetService();

            // act
            var result = await service.LoginAsync(_email, _password);
        
            // assert
           Assert.NotNull(result);
           Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public void GenerateJwtToken_Succeed()
        {
            //arrange
            var appSettings = new AppSettings
            {
                Jwt = new JwtSettings
                {
                    Audience = "audience",
                    Issuer = "issuer",
                    Key = "someSecureKeyWithAtLeast16Chars"
                },
                EncryptionKey = "encryptionKey"
            };
            _appSettings.Setup(x => x.Value).Returns(appSettings);
            var service = GetService();
            
            // act
            var result = service.GenerateJwtToken(new User
            {
                UserName = "test"
            });

            // assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        private LoginService GetService()
        {
            return new LoginService(_logger.Object, _signInManager.Object, _userManager.Object, _appSettings.Object);
        }
    }
}