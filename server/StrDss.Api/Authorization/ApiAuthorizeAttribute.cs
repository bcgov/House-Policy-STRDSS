using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StrDss.Common;

namespace StrDss.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public ApiAuthorizeAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult(); //403
                return;
            }

            if (_permissions.Length == 0)
            {
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
                context.Result = new UnauthorizedResult(); //401
                return;
            }
        }
    }
}
