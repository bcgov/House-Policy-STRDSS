using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace StrDss.Api.Authentication
{
    public class ApsJwtBearerEvents : JwtBearerEvents
    {
        private ICurrentUser _currentUser;
        private IUserService _userService;
        private IRoleService _roleService;
        private readonly ILogger<StrDssLogger> _logger;
        private readonly IMemoryCacheService _memoryCache;

        public ApsJwtBearerEvents(ICurrentUser currentUser, IUserService userService, IRoleService roleService, ILogger<StrDssLogger> logger, IMemoryCacheService memoryCache) : base()
        {
            _currentUser = currentUser;
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            _logger.LogDebug($"[AUTH] Token Validated with APS JWT");

            _currentUser.LoadApsSession(context!.Principal!);

            var (user, permissions) = await _userService.GetUserByDisplayNameAsync(_currentUser.DisplayName);

            if (user == null)
            {
                var errorMessage = $"{_currentUser.DisplayName} is not registered.";
                _logger.LogWarning(errorMessage);

                context.Response.StatusCode = 401;
                context.Fail($"Unauthorized: {errorMessage}");
                return;
            }

            _currentUser.Id = user.UserIdentityId;
            _currentUser.IsActive = user.IsEnabled;
            _currentUser.AccessRequestStatus = user.AccessRequestStatusCd;
            _currentUser.AccessRequestRequired = _currentUser.AccessRequestStatus == AccessRequestStatuses.Denied;
            _currentUser.OrganizationType = user.RepresentedByOrganization?.OrganizationType ?? "";
            _currentUser.OrganizationId = user.RepresentedByOrganizationId ?? 0;
            _currentUser.OrganizationName = user.RepresentedByOrganization?.OrganizationNm ?? "";
            _currentUser.TermsAcceptanceDtm = user.TermsAcceptanceDtm;

            if (user.IsEnabled)
            {
                _currentUser.Permissions = permissions;

                foreach (var permission in permissions)
                {
                    _currentUser.AddClaim(context!.Principal!, StrDssClaimTypes.Permission, permission);
                }
            }

            if (!context.Request.Headers.TryGetValue("GW-JWT", out var jwtToken))
            {
                _logger.LogWarning($"{_currentUser.UserName}: GW-JWT is missing");
                context.Response.StatusCode = 401;
                context.Fail("Unauthorized");
                return;
            }

            var publicKey = await _memoryCache.GetPublicKeyAsync(false);
            var gwJwt = jwtToken.ToString();

            if (gwJwt.IsEmpty())
            {
                _logger.LogWarning($"{_currentUser.UserName}: GW-JWT is empty");
                context.Response.StatusCode = 401;
                context.Fail("Unauthorized");
                return;
            }

            if (!ValidateJwtToken(gwJwt, publicKey))
            {
                publicKey = await _memoryCache.GetPublicKeyAsync(true);

                if (!ValidateJwtToken(gwJwt, publicKey))
                {
                    _logger.LogWarning($"{_currentUser.UserName}: GW-JWT is invalid");
                    context.Response.StatusCode = 401;
                    context.Fail("Unauthorized");
                    return;
                }
            }
        }

        private static bool ValidateJwtToken(string jwtToken, JsonWebKey publicKey)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new RsaSecurityKey(new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(publicKey.N),
                    Exponent = Base64UrlEncoder.DecodeBytes(publicKey.E)
                }),
                ValidateLifetime = true,
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken validatedToken;
                handler.ValidateToken(jwtToken, validationParameters, out validatedToken);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }
    }
}
