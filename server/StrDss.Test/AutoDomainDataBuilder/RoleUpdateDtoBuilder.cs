using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.UserDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class RoleUpdateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new RoleUpdateDtoBuilder());
        }
    }

    public class RoleUpdateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null)
                return new NoSpecimen();

            return pi.Name switch
            {
                nameof(RoleUpdateDto.UserRoleCd) => "admin",
                nameof(RoleUpdateDto.Permissions) => new List<string> { "read", "write" },
                _ => new NoSpecimen()
            };
        }
    }

}
