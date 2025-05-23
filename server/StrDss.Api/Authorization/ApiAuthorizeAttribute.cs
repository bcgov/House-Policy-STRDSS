﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Protocols.WsTrust;
using StrDss.Common;
using System.Security.Claims;

namespace StrDss.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions = null!;
        private ILogger<StrDssLogger> _logger = null!;

        public ApiAuthorizeAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<StrDssLogger>();

            var user = context.HttpContext.User;
            var username = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var ip = ipAddress == null ? "Unknown" : ipAddress.ToString();

            if (!user!.Identity!.IsAuthenticated)
            {
                _logger.LogInformation($"[AUTH] Authentication failed for user '{username}' at {context.ActionDescriptor.DisplayName} from IP address '{ip}'.");
                context.Result = new ForbidResult(); //403
                return;
            }

            var clientId = user.GetCustomClaim(StrDssClaimTypes.ClientId);

            var identityProviderNm = user.GetCustomClaim(StrDssClaimTypes.IdentityProvider);

            if (identityProviderNm == "" && clientId != "")
            {
                identityProviderNm = StrDssIdProviders.Aps;
                username = clientId;
            }

            var displayName = user.GetCustomClaim(StrDssClaimTypes.DisplayName);
            
            if (_permissions.Length == 0)
            {
                _logger.LogInformation($"[AUTH] User '{username}' is authorized to access {context.ActionDescriptor.DisplayName} from IP address {ip}.");
                return;
            }

            var hasPermission = false;

            foreach (var permission in _permissions)
            {
                if (user.HasClaim(StrDssClaimTypes.Permission, permission))
                {
                    hasPermission = true;
                    break;
                }
            }

            if (!hasPermission)
            {
                _logger.LogInformation($"[AUTH] User '{username}' does not have permission to access {context.ActionDescriptor.DisplayName} from IP address {ip}.");
                context.Result = new UnauthorizedResult(); //401
                return;
            }

            _logger.LogInformation($"[AUTH] User '{username}' is authorized to access {context.ActionDescriptor.DisplayName} from IP address {ip}.");
        }
    }
}
