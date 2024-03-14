using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Model;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class EmailContentCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new EmailContentBuilder());
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
}
