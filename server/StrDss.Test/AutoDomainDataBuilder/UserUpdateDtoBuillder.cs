using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.UserDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class UserUpdateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new UserUpdateDtoBuilder());
        }
    }

    public class UserUpdateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(UserUpdateDto))
                return new NoSpecimen();

            return pi.Name switch
            {
                nameof(UserUpdateDto.UserIdentityId) => 1,
                nameof(UserUpdateDto.RepresentedByOrganizationId) => 1,
                nameof(UserUpdateDto.RoleCds) => new List<string> { "admin", "user" },
                _ => new NoSpecimen()
            };
        }
    }
}
