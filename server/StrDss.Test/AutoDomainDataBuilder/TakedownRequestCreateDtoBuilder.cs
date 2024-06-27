using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Model.DelistingDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class TakedownRequestCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new TakedownRequestCreateDtoBuilder());
        }
    }

    public class TakedownRequestCreateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(TakedownRequestCreateDto))
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(TakedownRequestCreateDto.PlatformId):
                    return 1;
                case nameof(TakedownRequestCreateDto.ListingId):
                    return "1";
                case nameof(TakedownRequestCreateDto.ListingUrl):
                    return "https://example.com/listing";
                case nameof(TakedownRequestCreateDto.CcList):
                    return new List<string> { "cc1@example.com", "cc2@example.com" };
                default:
                    return new NoSpecimen();
            }
        }
    }
}
