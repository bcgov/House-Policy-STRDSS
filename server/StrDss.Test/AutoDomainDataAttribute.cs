using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using System.Reflection;

namespace StrDss.Test
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new EmailCustomization()))
        {
        }
    }

    public class EmailCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new SpecimenBuilder());
        }
    }

    public class SpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.PropertyType != typeof(string))
                return new NoSpecimen();

            switch (pi.Name.ToLower())
            {
                case "email":
                    return "user@user.com";
                case "streetaddress":
                    return "940 Blanshard St";
                case "city":
                    return "Victoria";
                case "province":
                    return "BC";
            }

            return new NoSpecimen();
        }
    }
}
