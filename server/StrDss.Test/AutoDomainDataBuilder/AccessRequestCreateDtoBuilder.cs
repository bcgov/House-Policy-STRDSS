using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.UserDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class AccessRequestCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new AccessRequestCreateDtoBuilder());
        }
    }

    public class AccessRequestCreateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi && pi.DeclaringType == typeof(AccessRequestCreateDto))
            {
                return pi.Name switch
                {
                    nameof(AccessRequestCreateDto.OrganizationType) => "Platform",
                    nameof(AccessRequestCreateDto.OrganizationName) => "Sample Organization",
                    _ => new NoSpecimen()
                };
            }

            return new NoSpecimen();
        }
    }
}
