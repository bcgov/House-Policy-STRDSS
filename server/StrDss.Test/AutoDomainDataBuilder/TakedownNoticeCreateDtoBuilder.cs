using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Model.DelistingDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class TakedownNoticeCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new TakedownNoticeCreateDtoBuilder());
        }
    }
    public class TakedownNoticeCreateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(TakedownNoticeCreateDto))
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(TakedownNoticeCreateDto.PlatformId):
                    return 1;
                case nameof(TakedownNoticeCreateDto.ListingId):
                    return "1";
                case nameof(TakedownNoticeCreateDto.ReasonId):
                    return 1;

                case nameof(TakedownNoticeCreateDto.ListingUrl):
                    return "https://example.com/listing";

                case nameof(TakedownNoticeCreateDto.HostEmail):
                    return "host@example.com";

                case nameof(TakedownNoticeCreateDto.HostEmailSent):
                    return false;

                case nameof(TakedownNoticeCreateDto.CcList):
                    return new List<string> { "cc1@example.com", "cc2@example.com" };

                case nameof(TakedownNoticeCreateDto.LgContactEmail):
                    return "lg@example.com";

                case nameof(TakedownNoticeCreateDto.LgContactPhone):
                    return "999-999-9999";

                case nameof(TakedownNoticeCreateDto.StrBylawUrl):
                    return "https://example.com/bylaw";

                case nameof(TakedownNoticeCreateDto.Comment):
                    return "This is a comment.";

                default:
                    return new NoSpecimen();
            }
        }
    }

}
