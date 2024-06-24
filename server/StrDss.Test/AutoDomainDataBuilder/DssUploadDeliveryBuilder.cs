using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Data.Entities;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class DssUploadDeliveryCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DssUploadDeliveryBuilder());
        }
    }

    public class DssUploadDeliveryBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(DssUploadDelivery))
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(DssUploadDelivery.UploadDeliveryId):
                    return 1;
                case nameof(DssUploadDelivery.UploadDeliveryType):
                    return "Type";
                case nameof(DssUploadDelivery.ReportPeriodYm):
                    return new DateOnly(2024, 3, 1);
                case nameof(DssUploadDelivery.SourceHashDsc):
                    return "Hash";
                case nameof(DssUploadDelivery.SourceBin):
                    return new byte[0];
                case nameof(DssUploadDelivery.ProvidingOrganizationId):
                    return 1;
                case nameof(DssUploadDelivery.UpdDtm):
                    return DateTime.Now;
                case nameof(DssUploadDelivery.UpdUserGuid):
                    return Guid.NewGuid();
                default:
                    return new NoSpecimen();
            }
        }
    }
}
