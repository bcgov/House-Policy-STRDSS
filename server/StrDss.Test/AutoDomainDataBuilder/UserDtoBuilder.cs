using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.UserDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class UserDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new UserDtoBuilder());
        }
    }

    public class UserDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(UserDto))
                return new NoSpecimen();

             switch (pi.Name)
            {
                case nameof(UserDto.UserIdentityId):
                    return 1;
                case nameof(UserDto.UserGuid):
                    return Guid.NewGuid();
                case nameof(UserDto.DisplayNm):
                    return "Test User";
                case nameof(UserDto.IdentityProviderNm):
                    return "idir";
                case nameof(UserDto.IsEnabled):
                    return true;
                case nameof(UserDto.AccessRequestStatusCd):
                    return "Requested";
                case nameof(UserDto.AccessRequestDtm):
                    return DateTime.UtcNow;
                case nameof(UserDto.AccessRequestJustificationTxt):
                    return "Test Justification";
                case nameof(UserDto.GivenNm):
                    return "Test";
                case nameof(UserDto.FamilyNm):
                    return "User";
                case nameof(UserDto.EmailAddressDsc):
                    return "test@example.com";
                case nameof(UserDto.BusinessNm):
                    return "Test Business";
                case nameof(UserDto.TermsAcceptanceDtm):
                    return DateTime.UtcNow;
                case nameof(UserDto.RepresentedByOrganizationId):
                    return null!;
                case nameof(UserDto.UpdDtm):
                    return DateTime.UtcNow;
                //case nameof(UserDto.UserRoleCds):
                //    return new List<RoleDto> { new RoleDto { UserRoleCd = "Role1" }, new RoleDto { UserRoleCd = "Role2" } };
                default:
                    return new NoSpecimen();
            }
        }
    }

}
