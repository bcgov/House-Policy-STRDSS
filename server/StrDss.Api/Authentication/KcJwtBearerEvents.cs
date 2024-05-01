using Microsoft.AspNetCore.Authentication.JwtBearer;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;
using StrDss.Service.Bceid;

namespace StrDss.Api.Authentication
{
    public class KcJwtBearerEvents : JwtBearerEvents
    {
        private ICurrentUser _currentUser;
        private IUserService _userService;
        private IBceidApi _bceid;
        private ILogger<StrDssLogger> _logger;

        public KcJwtBearerEvents(ICurrentUser currentUser, IUserService userService, IBceidApi bceid, ILogger<StrDssLogger> logger) : base()
        {
            _currentUser = currentUser;
            _userService = userService;
            _bceid = bceid;
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
            _currentUser.LoadUserSession(context!.Principal!);

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
                        _currentUser.AddClaim(context!.Principal!, StrDssClaimTypes.Permission, permission);
                    }
                }

                if (user.IdentityProviderNm == StrDssIdProviders.BceidBusiness && (int)(DateTime.UtcNow - user!.UpdDtm).TotalDays > 1)
                {
                    try
                    {
                        var (error, account) = await _bceid.GetBceidAccountCachedAsync(_currentUser.UserGuid, "", StrDssIdProviders.BceidBusiness, _currentUser.UserGuid, _currentUser.IdentityProviderNm);

                        if (account == null)
                        {
                            _logger.LogError($"BCeID call error: {error}");
                        }

                        if (account != null)
                        {
                            _currentUser.FirstName = account.FirstName;
                            _currentUser.LastName = account.LastName;

                            await _userService.UpdateBceidUserInfo(user.UserIdentityId, account.FirstName, account.LastName);
                        }
                    }
                    catch
                    {
                        _logger.LogInformation("BCeID Web call failed - Skipping UpdateBceidUserInfo ");
                    }
                }
            }
        }
    }
}
