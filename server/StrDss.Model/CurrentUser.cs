using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using StrDss.Common;

namespace StrDss.Model
{
    public interface ICurrentUser
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public Guid UserGuid { get; set; }
        public string UserType { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public string AccessRequestStatus { get; set; }

        void LoadUserSession(ClaimsPrincipal user);
        void AddClaim(ClaimsPrincipal user, string claimType, string value);
    }

    public class CurrentUser : ICurrentUser
    {
        public long Id { get; set; }
        public string UserName { get; set; } = "";
        [JsonIgnore]
        public Guid UserGuid { get; set; }
        public string UserType { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public string AccessRequestStatus { get; set; } = "";

        public void LoadUserSession(ClaimsPrincipal user)
        {
            if (user == null)
                return;

            var textInfo = new CultureInfo("en-US", false).TextInfo;

            UserType = user.GetCustomClaim(StrDssClaimTypes.IdentityProvider);
            EmailAddress = user.GetCustomClaim(ClaimTypes.Email);
            FirstName = textInfo.ToTitleCase(user.GetCustomClaim(ClaimTypes.GivenName));
            LastName = textInfo.ToTitleCase(user.GetCustomClaim(ClaimTypes.Surname));
            DisplayName = user.GetCustomClaim(StrDssClaimTypes.DisplayName);

            switch (UserType)
            {
                case StrDssUserTypes.IDIR:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.IdirUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.IdirUsername);
                    break;
                case StrDssUserTypes.BceidBusiness:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.BceidUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.BceidUsername);
                    break;
                case StrDssUserTypes.StrDss:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.StrDssUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.StrDssUsername);
                    break;
                default:
                    UserGuid = Guid.Empty;
                    break;
            }

            FullName = CommonUtils.GetFullName(FirstName, LastName);
        }

        public void AddClaim(ClaimsPrincipal user, string claimType, string value)
        {
            if (user == null || claimType.IsEmpty() || value.IsEmpty() || user.HasClaim(claimType, value)) return;

            user.Identities.FirstOrDefault().AddClaim(new Claim(claimType, value));
        }
    }
}
