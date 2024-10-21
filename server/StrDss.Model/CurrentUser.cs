using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using NetTopologySuite.Noding;
using StrDss.Common;

namespace StrDss.Model
{
    public interface ICurrentUser
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public Guid UserGuid { get; set; }
        public string IdentityProviderNm { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public string BusinessNm { get; set; }
        public string AccessRequestStatus { get; set; }
        public bool AccessRequestRequired { get; set; }
        public List<string> Permissions { get; set; }
        public string OrganizationType { get; set; }
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime? TermsAcceptanceDtm { get; set; }
        void LoadUserSession(ClaimsPrincipal user);
        void LoadApsSession(ClaimsPrincipal user);
        void AddClaim(ClaimsPrincipal user, string claimType, string value);
        string? ExternalIdentityCd { get; set; }
        bool IsBcServicesCard { get; set; }
    }

    public class CurrentUser : ICurrentUser
    {
        public long Id { get; set; }
        public string UserName { get; set; } = "";
        [JsonIgnore]
        public Guid UserGuid { get; set; }
        public string IdentityProviderNm { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public string BusinessNm { get; set; } = "";
        public string AccessRequestStatus { get; set; } = "";
        public bool AccessRequestRequired { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public string OrganizationType { get; set; } = "";
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; } = "";
        public DateTime? TermsAcceptanceDtm { get; set; }
        public string? ExternalIdentityCd { get; set; }
        public bool IsBcServicesCard { get; set; } = false;

        public void LoadUserSession(ClaimsPrincipal user)
        {
            if (user == null)
                return;

            var textInfo = new CultureInfo("en-US", false).TextInfo;

            IdentityProviderNm = user.GetCustomClaim(StrDssClaimTypes.IdentityProvider);
            EmailAddress = user.GetCustomClaim(ClaimTypes.Email);
            FirstName = textInfo.ToTitleCase(user.GetCustomClaim(ClaimTypes.GivenName));
            LastName = textInfo.ToTitleCase(user.GetCustomClaim(ClaimTypes.Surname));
            DisplayName = user.GetCustomClaim(StrDssClaimTypes.DisplayName);
            ExternalIdentityCd = user.GetCustomClaim(StrDssClaimTypes.Sub);
            IsBcServicesCard = ExternalIdentityCd == Environment.GetEnvironmentVariable("SSO_CLIENT");

            switch (IdentityProviderNm)
            {
                case StrDssIdProviders.Idir:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.IdirUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.IdirUsername);
                    break;
                case StrDssIdProviders.BceidBusiness:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.BceidUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.BceidUsername);
                    BusinessNm = user.GetCustomClaim(StrDssClaimTypes.BceidBusinessName);
                    break;
                case StrDssIdProviders.StrDss:
                    UserGuid = new Guid(user.GetCustomClaim(StrDssClaimTypes.StrDssUserGuid));
                    UserName = user.GetCustomClaim(StrDssClaimTypes.StrDssUsername);
                    break;
                default:
                    UserGuid = Guid.Empty;
                    break;
            }

            FullName = CommonUtils.GetFullName(FirstName, LastName);
        }

        public void LoadApsSession(ClaimsPrincipal user)
        {
            if (user == null)
                return;

            var textInfo = new CultureInfo("en-US", false).TextInfo;

            IdentityProviderNm = StrDssIdProviders.Aps;
            DisplayName = user.GetCustomClaim(StrDssClaimTypes.ClientId);
        }

        public void AddClaim(ClaimsPrincipal user, string claimType, string value)
        {
            if (user == null || claimType.IsEmpty() || value.IsEmpty() || user.HasClaim(claimType, value)) return;

            user!.Identities!.FirstOrDefault()!.AddClaim(new Claim(claimType, value));
        }
    }
}
