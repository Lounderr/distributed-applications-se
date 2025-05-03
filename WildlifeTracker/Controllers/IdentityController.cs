using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WildlifeTracker.Constants;
using WildlifeTracker.Data.Models;
using WildlifeTracker.Exceptions;
using WildlifeTracker.Models;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
                              TimeProvider timeProvider) : ControllerBase
    {
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomRegisterRequest registration)
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("User store must support email");
            }

            if (string.IsNullOrEmpty(registration.Email) || !_emailAddressAttribute.IsValid(registration.Email))
            {
                throw new ServiceException(ErrorCodes.EmailInvalid, "The email provided is invalid");
            }

            var user = new User
            {
                UserName = registration.Email,
                Email = registration.Email,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                DateOfBirth = registration.DateOfBirth,
                City = registration.City
            };

            var result = await userManager.CreateAsync(user, registration.Password);
            if (!result.Succeeded)
            {
                throw new ServiceException(result);
            }

            return this.Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomLoginRequest login)
        {
            signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                throw new UnauthorizedException(result.ToString());
            }

            return this.Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                timeProvider.GetUtcNow() >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)
            {
                throw new UnauthorizedException("Refresh token is invalid");
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return this.SignIn(newPrincipal, IdentityConstants.BearerScheme);
        }
    }
}
