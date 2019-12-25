using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Helpers;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Interfaces.Services;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Core.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Galaxy.Auth.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserManager<User> _userManager;
        private readonly IInvitationTokenRepository _invitationTokenRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUrlEncoder _urlEncoder;
        private readonly ISignInManager<User> _signInManager;
        private readonly AppSettings _appSettings;

        public UserService(ILogger<UserService> logger, ISignInManager<User> signInManager, IUserManager<User> userManager, IOptions<AppSettings> appSettings, 
            IInvitationTokenRepository invitationTokenRepository, IEmailSender emailSender, IUrlEncoder urlEncoder)
        {
            _logger = logger;
            _userManager = userManager;
            _invitationTokenRepository = invitationTokenRepository;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }
        public async Task<ActionResponse> RegisterAsync(RegisterModel model)
        {
            var user = new User
            {
                Email = model.Email,
                Name = model.Name,
                UserName = Guid.NewGuid().ToString()
            };
            var savedUser = await _userManager.CreateAsync(user, model.Password);

            if (!savedUser.Succeeded)
            {
                _logger.LogCritical($"User creation failed {JsonConvert.SerializeObject(savedUser.Errors)}");
                return ToActionResponse(savedUser.Errors);
            }

            var token = new InvitationToken
            {
                Email = model.Email,
                Token = Encryptor.EncryptText(model.Email, _appSettings.EncryptionKey)
            };
            
            await _invitationTokenRepository.AddAsync(token);
            _emailSender.SendEmail(model.Email, $"Hello {model.Name}, <br /> In order to activate your " +
                                                $"account please click here: https://localhost:5003/user/activate?activationToken={_urlEncoder.Encode(token.Token)}" +
                                                $"<br/> <br/>Thank you, <br/>Galaxy team",
                "Please activate your account");
            return new ActionResponse();
        }

        public async Task<ActionResponse> ActivateAsync(string token)
        {
            await Task.CompletedTask;
            string decodedToken;
            try
            {
                decodedToken = _urlEncoder.Decode(token);
            }
            catch
            {
                _logger.LogWarning("invalid activation token was requested");
                return ActionResponse.UnauthorizedAccess();
            }
            
            if(string.IsNullOrEmpty(decodedToken))
                return ActionResponse.UnauthorizedAccess();

            var dbToken = await _invitationTokenRepository.GetAsync(decodedToken);
            if(dbToken == null) return ActionResponse.UnauthorizedAccess();

            var user = await _userManager.FindByEmailAsync(dbToken.Email);
            if(user == null) return ActionResponse.UnauthorizedAccess();
            
            // activate user
            var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmToken);

            if (!result.Succeeded)
            {
                _logger.LogCritical($"User activation failed: {result.Errors}");
                return ToActionResponse(result.Errors);
            }

            await _invitationTokenRepository.RemoveAsync(dbToken);
            return new ActionResponse();
        }

        public async Task<ActionResponse> UpdateAsync(string username, string name, string phone)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return ActionResponse.UnauthorizedAccess();
            user.Name =  string.IsNullOrEmpty(name)? user.Name: name;
            user.PhoneNumber = phone;

            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded) return new ActionResponse();
            _logger.LogError($"Failed to update user:{username} with errors: {JsonConvert.SerializeObject(result.Errors)}");
            return ToActionResponse(result.Errors);
        }

        public async Task<ActionResponse> ChangePasswordAsync(string username, string newPassword, string oldPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null || !user.EmailConfirmed) return ActionResponse.UnauthorizedAccess();
            var signIn = await _signInManager.PasswordSignInAsync(username, oldPassword, false, false);
            if (!signIn.Succeeded)  return ActionResponse.UnauthorizedAccess();
            
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (result.Succeeded) return new ActionResponse();
            _logger.LogCritical(
                $"Change password failed for user: {username}. Errors: {JsonConvert.SerializeObject(result.Errors)}");
            return ToActionResponse(result.Errors);
        }

        public async Task<User> VerifyUserExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
        
        private ActionResponse ToActionResponse(IEnumerable<IdentityError> errors)
        {
            var identityErrors = errors.ToList();
            var response = new ActionResponse {Success = !identityErrors.Any()};
            
            identityErrors.ForEach(err =>
            {
                response.Errors.Add(new ActionError
                {
                    Code = err.Code,
                    Description = err.Description
                });
            });
            return response;
        }
    }
}