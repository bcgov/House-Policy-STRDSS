﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;

namespace StrDss.Api.Authentication
{
    public class KcJwtBearerEvents : JwtBearerEvents
    {
        private ICurrentUser _currentUser;
        private IUserService _userService;

        public KcJwtBearerEvents(ICurrentUser currentUser, IUserService userService) : base()
        {
            _currentUser = currentUser;
            _userService = userService;
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
