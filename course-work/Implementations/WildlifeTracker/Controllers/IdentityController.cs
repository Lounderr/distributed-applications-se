using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.Annotations;

using WildlifeTracker.Constants;
using WildlifeTracker.Data.Models;
using WildlifeTracker.Exceptions;
using WildlifeTracker.Helpers;
using WildlifeTracker.Helpers.DataAnotations;
using WildlifeTracker.Models.IdentityDtos;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("User authentication and registration operations")]
    public class IdentityController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
                              TimeProvider timeProvider) : ControllerBase
    {
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register new user",
            Description = "Creates a new user account with the provided registration information",
            OperationId = "Register",
            Tags = new[] { "Identity" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "User successfully registered")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid registration data")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "User with this email already exists")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registration)
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("User store must support email");
            }

            if (string.IsNullOrEmpty(registration.Email) || !_emailAddressAttribute.IsValid(registration.Email))
            {
                throw new ServiceException(ErrorCodes.EmailInvalid, "The email provided is invalid");
            }

            E164FormatValidatorAttribute phoneValidator = new();

            bool isValidPhone = phoneValidator.IsValid(registration.PhoneNumber);

            if (!isValidPhone)
            {
                throw new ServiceException(ErrorCodes.MobileInvalid, phoneValidator.ErrorMessage ?? "Phone number is not valid");
            }

            var user = new User
            {
                UserName = registration.Email,
                Email = registration.Email,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PhoneNumber = MobileToE164.Convert(registration.PhoneNumber),
                City = string.IsNullOrWhiteSpace(registration.City) ? null : registration.City,
                DateOfBirth = registration.DateOfBirth,
            };

            var result = await userManager.CreateAsync(user, registration.Password);
            if (!result.Succeeded)
            {
                throw new ServiceException(result);
            }

            return this.Ok();
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticates a user and returns a bearer token for subsequent requests",
            OperationId = "Login",
            Tags = new[] { "Identity" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully authenticated")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials")]
        [SwaggerResponse(StatusCodes.Status423Locked, "Account is locked")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
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
        [SwaggerOperation(
            Summary = "Refresh token",
            Description = "Refreshes an expired bearer token using a valid refresh token",
            OperationId = "RefreshToken",
            Tags = new[] { "Identity" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Token successfully refreshed")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired refresh token")]
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
