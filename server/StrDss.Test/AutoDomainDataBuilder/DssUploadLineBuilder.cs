using AutoFixture;
using AutoFixture.Kernel;
using StrDss.Data.Entities;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class DssUploadLineCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DssUploadLineBuilder());
        }
    }

    public class DssUploadLineBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(DssUploadLine))
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(DssUploadLine.UploadLineId):
                    return 1;
                case nameof(DssUploadLine.IsValidationFailure):
                    return false;
                case nameof(DssUploadLine.IsSystemFailure):
                    return false;
                case nameof(DssUploadLine.IsProcessed):
                    return false;
                case nameof(DssUploadLine.SourceOrganizationCd):
                    return "ORG";
                case nameof(DssUploadLine.SourceRecordNo):
                    return "123";
                case nameof(DssUploadLine.SourceLineTxt):
                    return "Sample text";
                case nameof(DssUploadLine.ErrorTxt):
                    return "";
                case nameof(DssUploadLine.IncludingUploadDeliveryId):
                    return 1;
                case nameof(DssUploadLine.IncludingUploadDelivery):
                    return new DssUploadDelivery
                    {
                        UploadDeliveryId = 1
                    };
                default:
                    return new NoSpecimen();
            }
        }
    }
}
