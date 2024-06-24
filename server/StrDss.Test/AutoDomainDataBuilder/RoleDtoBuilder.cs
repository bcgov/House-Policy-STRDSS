using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.UserDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class RoleDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new RoleDtoBuilder());
        }
    }

    public class RoleDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(RoleDto))
                return new NoSpecimen();

            return pi.Name switch
            {
                nameof(RoleDto.UserRoleCd) => "admin",
                nameof(RoleDto.Permissions) => context.CreateMany<PermissionDto>(2).ToList(),
                nameof(RoleDto.IsReferenced) => true,
                _ => new NoSpecimen()
            };
        }
    }
}
