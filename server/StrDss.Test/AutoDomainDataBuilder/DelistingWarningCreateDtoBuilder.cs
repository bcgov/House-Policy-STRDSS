using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Model.DelistingDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class DelistingWarningCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DelistingWarningCreateDtoBuilder());
        }
    }
    public class DelistingWarningCreateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null)
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(DelistingWarningCreateDto.PlatformId):
                    return 1;
                case nameof(DelistingWarningCreateDto.ListingId):
                    return 1;
                case nameof(DelistingWarningCreateDto.ReasonId):
                    return 1;

                case nameof(DelistingWarningCreateDto.ListingUrl):
                    return "https://example.com/listing";

                case nameof(DelistingWarningCreateDto.HostEmail):
                    return "host@example.com";

                case nameof(DelistingWarningCreateDto.HostEmailSent):
                    return false;

                case nameof(DelistingWarningCreateDto.SendCopy):
                    return true;

                case nameof(DelistingWarningCreateDto.CcList):
                    return new List<string> { "cc1@example.com", "cc2@example.com" };

                case nameof(DelistingWarningCreateDto.LgContactEmail):
                    return "lg@example.com";

                case nameof(DelistingWarningCreateDto.LgContactPhone):
                    return "(123) 456-7890";

                case nameof(DelistingWarningCreateDto.StrBylawUrl):
                    return "https://example.com/bylaw";

                case nameof(DelistingWarningCreateDto.Comment):
                    return "This is a comment.";

                default:
                    return new NoSpecimen();
            }
        }
    }

}
