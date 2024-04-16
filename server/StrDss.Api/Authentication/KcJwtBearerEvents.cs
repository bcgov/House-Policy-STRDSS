using Microsoft.AspNetCore.Authentication.JwtBearer;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;

namespace StrDss.Api.Authentication
{
    public class KcJwtBearerEvents : JwtBearerEvents
    {
        private ICurrentUser _currentUser;
        private IUserService _userService;
        private ILogger<StrDssLogger> _logger;

        public KcJwtBearerEvents(ICurrentUser currentUser, IUserService userService, ILogger<StrDssLogger> logger) : base()
        {
            _currentUser = currentUser;
            _userService = userService;
            _logger = logger;
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            var username = context.HttpContext.User?.Identity?.Name ?? "Unknown";
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var ip = ipAddress == null ? "Unknown" : ipAddress.ToString();

            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogWarning($"[AUTH] Authentication failed for user '{username}' from IP address '{ip}'. Authorization header is missing.");
            }

            return base.Challenge(context);
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            var username = context.HttpContext.User?.Identity?.Name ?? "Unknown";
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var ip = ipAddress == null ? "Unknown" : ipAddress.ToString();

            _logger.LogInformation($"[AUTH] Authentication failed for user '{username}' from IP address '{ip}'.");

            return base.AuthenticationFailed(context);
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            _currentUser.LoadUserSession(context.Principal);

            var (user, permissions) = await _userService.GetUserByGuidAsync(_currentUser.UserGuid);

            if (user == null)
            {
                _currentUser.AccessRequestStatus = AccessRequestStatuses.None;
                _currentUser.IsActive = false;
                _currentUser.AccessRequestRequired = true;
            }
            else
            {
                _currentUser.Id = user.UserIdentityId;
                _currentUser.IsActive = user.IsEnabled;
                _currentUser.AccessRequestStatus = user.AccessRequestStatusCd;
                _currentUser.AccessRequestRequired = _currentUser.AccessRequestStatus == AccessRequestStatuses.Denied;
                _currentUser.OrganizationType = user.RepresentedByOrganization?.OrganizationType ?? "";
                _currentUser.OrganizationId = user.RepresentedByOrganizationId ?? 0;
                _currentUser.TermsAcceptanceDtm = user.TermsAcceptanceDtm;

                if (user.IsEnabled)
                {
                    _currentUser.Permissions = permissions;

                    foreach (var permission in permissions)
                    {
                        _currentUser.AddClaim(context.Principal, StrDssClaimTypes.Permission, permission);
                    }
                }
            }
        }
    }
}
