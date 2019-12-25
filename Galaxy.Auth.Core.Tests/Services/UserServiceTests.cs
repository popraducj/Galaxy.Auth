using System;
using System.Linq;
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
    public class UserServiceTests
    {
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<IUserManager<User>> _userManager;
        private readonly Mock<IInvitationTokenRepository> _invitationTokenRepository;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<IUrlEncoder> _urlEncoder;
        private readonly Mock<ISignInManager<User>> _signInManager;
        private readonly Mock<IOptions<AppSettings>> _appSettings;
        private const string ErrorMessage = "Something went wrong";
        private const string UnauthorizedAccess = "You are not authorized to access this resource";

        public UserServiceTests()
        {
            _logger = new Mock<ILogger<UserService>>();
            _userManager = new Mock<IUserManager<User>>();
            _invitationTokenRepository = new Mock<IInvitationTokenRepository>();
            _emailSender = new Mock<IEmailSender>();
            _urlEncoder = new Mock<IUrlEncoder>();
            _signInManager = new Mock<ISignInManager<User>>();
            _appSettings = new Mock<IOptions<AppSettings>>();
        }

        [Fact]
        public async Task RegisterAsync_CreationFail_ReturnErrors()
        {
            // arrange
            var model = new RegisterModel
            {
                Email = "test@test.com",
                Name = "Some random name",
                Password = "Test123!"
            };

            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = ErrorMessage}));
            var service = GetService();
            // act
            
            var result = await service.RegisterAsync(model);
            // assert
            
            Assert.False(result.Success);
            Assert.Equal(ErrorMessage, result.Errors.First().Description);
        }
        
        

        [Fact]
        public async Task RegisterAsync_Succeed()
        {
            // arrange
            var model = new RegisterModel
            {
                Email = "test@test.com",
                Name = "Some random name",
                Password = "Test123!"
            };
            var appSettings = new AppSettings
            {
                EncryptionKey = "encryptionKey"
            };

            _appSettings.Setup(x => x.Value).Returns(appSettings);
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _invitationTokenRepository.Setup(x => x.AddAsync(It.IsAny<InvitationToken>())).Returns(Task.CompletedTask);
            _emailSender.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var service = GetService();
            // act
            
            var result = await service.RegisterAsync(model);
            // assert
            
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ActivateAsync_InvalidTokenException_ReturnUnauthorized()
        {
            // arrange

            const string token = " some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Throws<Exception>();
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ActivateAsync_InvalidToken_ReturnUnauthorized()
        {
            // arrange
            const string token = " some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Returns((string)null);
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ActivateAsync_TokenNotFoundInDb_ReturnUnauthorized()
        {
            // arrange

            const string token = "some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Returns(token);
            _invitationTokenRepository.Setup(x => x.GetAsync(token)).ReturnsAsync((InvitationToken) null);
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ActivateAsync_UserNotFound_ReturnUnauthorized()
        {
            // arrange

            const string token = "some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Returns(token);
            _invitationTokenRepository.Setup(x => x.GetAsync(token)).ReturnsAsync(new InvitationToken());
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ActivateAsync_FailedToConfirmEmail_ReturnErrors()
        {
            // arrange

            const string token = "some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Returns(token);
            _invitationTokenRepository.Setup(x => x.GetAsync(token)).ReturnsAsync(new InvitationToken());
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync(string.Empty);
            _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = ErrorMessage}));
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(ErrorMessage, result.Errors.First().Description);
        }
        
        
        [Fact]
        public async Task ActivateAsync_Succeed()
        {
            // arrange

            const string token = "some random token";
            _urlEncoder.Setup(x => x.Decode(token)).Returns(token);
            _invitationTokenRepository.Setup(x => x.GetAsync(token)).ReturnsAsync(new InvitationToken());
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync(string.Empty);
            _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _invitationTokenRepository.Setup(x => x.RemoveAsync(It.IsAny<InvitationToken>()))
                .Returns(Task.CompletedTask);
            var service = GetService();

            // act
            var result = await service.ActivateAsync(token);

            // assert
            
            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpdateAsync_UsernameNotFound_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string name = "new name";
            const string phone = "new phone";

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync((User) null);
            var service = GetService();
            
            // act
            var result = await service.UpdateAsync(username, name, phone);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }

        [Fact]
        public async Task UpdateAsync_FailedToUpdate_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string name = "new name";
            const string phone = "new phone";

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(new User());
            _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = ErrorMessage}));
            var service = GetService();
            
            // act
            var result = await service.UpdateAsync(username, name, phone);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(ErrorMessage, result.Errors.First().Description);
        }

        [Fact]
        public async Task UpdateAsync_Succeed()
        {
            // arrange

            const string username = "guid";
            const string name = "new name";
            const string phone = "new phone";

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(new User());
            _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);
            var service = GetService();
            
            // act
            var result = await service.UpdateAsync(username, name, phone);

            // assert
            
            Assert.True(result.Success);
        }
        
        [Fact]
        public async Task ChangePasswordAsync_UsernameNotFound_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string newPassword = "new password";
            const string oldPassword = "old password";

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync((User) null);
            var service = GetService();
            
            // act
            var result = await service.ChangePasswordAsync(username, newPassword, oldPassword);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ChangePasswordAsync_UsernameNotConfirmed_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string newPassword = "new password";
            const string oldPassword = "old password";

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(new User());
            var service = GetService();
            
            // act
            var result = await service.ChangePasswordAsync(username, newPassword, oldPassword);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ChangePasswordAsync_SignInFailed_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string newPassword = "new password";
            const string oldPassword = "old password";
            var user = new User
            {
                UserName = "user",
                EmailConfirmed = true
            };

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _signInManager.Setup(x => x.PasswordSignInAsync(username, oldPassword, false, false))
                .ReturnsAsync(SignInResult.Failed);
            var service = GetService();
            
            // act
            var result = await service.ChangePasswordAsync(username, newPassword, oldPassword);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(UnauthorizedAccess, result.Errors.First().Description);
        }
        
        [Fact]
        public async Task ChangePasswordAsync_FailedToChangePassword_ReturnsUnauthorized()
        {
            // arrange

            const string username = "guid";
            const string newPassword = "new password";
            const string oldPassword = "old password";
            var user = new User
            {
                UserName = "user",
                EmailConfirmed = true
            };

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _signInManager.Setup(x => x.PasswordSignInAsync(username, oldPassword, false, false))
                .ReturnsAsync(SignInResult.Success);
            _userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(string.Empty);
            _userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), newPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = ErrorMessage}));
            var service = GetService();
            
            // act
            var result = await service.ChangePasswordAsync(username, newPassword, oldPassword);

            // assert
            
            Assert.False(result.Success);
            Assert.Equal(ErrorMessage, result.Errors.First().Description);
        }
        
        
        
        [Fact]
        public async Task ChangePasswordAsyncSucceed()
        {
            // arrange

            const string username = "guid";
            const string newPassword = "new password";
            const string oldPassword = "old password";
            var user = new User
            {
                UserName = "user",
                EmailConfirmed = true
            };

            _userManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _signInManager.Setup(x => x.PasswordSignInAsync(username, oldPassword, false, false))
                .ReturnsAsync(SignInResult.Success);
            _userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(string.Empty);
            _userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), newPassword))
                .ReturnsAsync(IdentityResult.Success);
            var service = GetService();
            
            // act
            var result = await service.ChangePasswordAsync(username, newPassword, oldPassword);

            // assert
            
            Assert.True(result.Success);
        }

        private UserService GetService()
        {
            return new UserService(_logger.Object, _signInManager.Object, _userManager.Object, _appSettings.Object,
                _invitationTokenRepository.Object, _emailSender.Object, _urlEncoder.Object);
        }
    }
}