using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Common;
using StrDss.Model.OrganizationDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class OrganizationDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new OrganizationDtoBuilder());
        }
    }
    public class OrganizationDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null)
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(OrganizationDto.OrganizationId):
                    return 2;
                case nameof(OrganizationDto.OrganizationType):
                    return OrganizationTypes.Platform;
                case nameof(OrganizationDto.OrganizationCd):
                    return "PLATFORMTEST";
                case nameof(OrganizationDto.OrganizationNm):
                    return "Test Platform";
                case nameof(OrganizationDto.LocalGovernmentGeometry):
                    return null!;
                case nameof(OrganizationDto.ManagingOrganizationId):
                    return null!;
                case nameof(OrganizationDto.ContactPeople):
                    return new List<ContactPersonDto> 
                    { 
                        new ContactPersonDto
                        {
                            OrganizationContactPersonId = 1,
                            IsPrimary = true,
                            GivenNm = "John",
                            FamilyNm = "Doe",
                            PhoneNo = "999-999-9999",
                            EmailAddressDsc = "foo@foo.com",
                            ContactedThroughOrganizationId = 0,
                            EmailMessageType = EmailMessageTypes.TakedownRequest
                        },
                        new ContactPersonDto
                        {
                            OrganizationContactPersonId = 1,
                            IsPrimary = true,
                            GivenNm = "John",
                            FamilyNm = "Doe",
                            PhoneNo = "999-999-9999",
                            EmailAddressDsc = "foo@foo.com",
                            ContactedThroughOrganizationId = 0,
                            EmailMessageType = EmailMessageTypes.BatchTakedownRequest
                        }
                    };
                default:
                    return new NoSpecimen();
            }
        }
    }

}
