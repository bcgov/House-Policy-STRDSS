using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using StrDss.Model;
using StrDss.Model.DelistingDtos;

namespace StrDss.Test
{
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
                    return "1234567890";

                case nameof(DelistingWarningCreateDto.StrBylawUrl):
                    return "https://example.com/bylaw";

                case nameof(DelistingWarningCreateDto.Comment):
                    return "This is a comment.";

                default:
                    return new NoSpecimen();
            }
        }
    }

    public class DelistingRequestCreateDtoBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null)
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(DelistingRequestCreateDto.PlatformId):
                    return 1;
                case nameof(DelistingRequestCreateDto.ListingId):
                    return 1;
                case nameof(DelistingRequestCreateDto.LgId):
                    return 1;

                case nameof(DelistingRequestCreateDto.ListingUrl):
                    return "https://example.com/listing";

                case nameof(DelistingRequestCreateDto.SendCopy):
                    return true;

                case nameof(DelistingRequestCreateDto.CcList):
                    return new List<string> { "cc1@example.com", "cc2@example.com" };

                default:
                    return new NoSpecimen();
            }
        }
    }

    public class EmailContentBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is Type type) || type != typeof(EmailContent))
            {
                return new NoSpecimen();
            }

            var emailContent = new EmailContent
            {
                Bcc = context.Create<string[]>(),
                BodyType = context.Create<string>(),
                Body = context.Create<string>(),
                Cc = context.Create<string[]>(),
                DelayTS = context.Create<int>(),
                Encoding = context.Create<string>(),
                From = context.Create<string>(),
                Priority = context.Create<string>(),
                Subject = context.Create<string>(),
                To = context.Create<string[]>(),
                Info = context.Create<string>()
            };

            return emailContent;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new DelistingWarningCreateDtoCustomization())
                .Customize(new DelistingRequestCreateDtoCustomization())
                .Customize(new EmailContentCustomization())
            )                
        {
        }
    }

    public class DelistingWarningCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DelistingWarningCreateDtoBuilder());
        }
    }

    public class DelistingRequestCreateDtoCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DelistingRequestCreateDtoBuilder());
        }
    }

    public class EmailContentCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new EmailContentBuilder());
        }
    }
}
