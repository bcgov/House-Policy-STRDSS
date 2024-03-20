using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using StrDss.Test.AutoDomainDataBuilder;

namespace StrDss.Test
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new DelistingWarningCreateDtoCustomization())
                .Customize(new DelistingRequestCreateDtoCustomization())
                .Customize(new EmailContentCustomization())
                .Customize (new OrganizationDtoCustomization())
            )                
        {
        }
    }
}
