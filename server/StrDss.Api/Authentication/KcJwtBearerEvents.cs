using Microsoft.AspNetCore.Authentication.JwtBearer;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.UserDtos;
using StrDss.Service;
using StrDss.Service.Bceid;
using System.Net;

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

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            _logger.LogDebug($"[AUTH] Token Validated with KC JWT");

            _currentUser.LoadUserSession(context!.Principal!);

            var (user, permissions) = await _userService.GetUserByCurrentUserAsync();

            if (user == null)
            {
                _currentUser.AccessRequestStatus = AccessRequestStatuses.None;
                _currentUser.IsActive = false;
                _currentUser.AccessRequestRequired = true;
            }
            else
            {
                _currentUser.UserGuid = user.UserGuid;
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

                //await UpdateBceidUserInfo(user);
            }
        }

        private async Task UpdateBceidUserInfo(UserDto? user)
        {
            if (user!.IdentityProviderNm == StrDssIdProviders.BceidBusiness && (int)(DateTime.UtcNow - user!.UpdDtm).TotalDays > 1)
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

                        if (account.FirstName != user.GivenNm || account.LastName != user.FamilyNm)
                        {
                            await _userService.UpdateBceidUserInfo(user.UserIdentityId, account.FirstName, account.LastName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"BCeID Web call failed - {ex.Message}", ex);
                    _logger.LogInformation("BCeID Web call failed - Skipping UpdateBceidUserInfo ");
                }
            }
        }
    }
}
