using Microsoft.AspNetCore.Authentication.JwtBearer;
using StrDss.Model;

namespace StrDss.Api.Authentication
{
    public class KcJwtBearerEvents : JwtBearerEvents
    {
        private ICurrentUser _currentUser;

        public KcJwtBearerEvents(ICurrentUser currentUser) : base()
        {
            _currentUser = currentUser;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            _currentUser.LoadUserSession(context.Principal);

            await Task.CompletedTask;
        }
    }
}
