using AutoFixture.Kernel;
using AutoFixture;
using StrDss.Model.RentalReportDtos;
using System.Reflection;

namespace StrDss.Test.AutoDomainDataBuilder
{
    public class RentalListingRowUntypedCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new RentalListingRowUntypedBuilder());
        }
    }

    public class RentalListingRowUntypedBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as PropertyInfo;

            if (pi == null || pi.DeclaringType != typeof(RentalListingRowUntyped))
                return new NoSpecimen();

            switch (pi.Name)
            {
                case nameof(RentalListingRowUntyped.RptPeriod):
                    return "2024-05";
                case nameof(RentalListingRowUntyped.OrgCd):
                    return "org1";
                case nameof(RentalListingRowUntyped.ListingId):
                    return "1";
                case nameof(RentalListingRowUntyped.ListingUrl):
                    return "https://example.com";
                case nameof(RentalListingRowUntyped.RentalAddress):
                    return "525 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.BusLicNo):
                    return "ABC123";
                case nameof(RentalListingRowUntyped.BcRegNo):
                    return "BCR456";
                case nameof(RentalListingRowUntyped.IsEntireUnit):
                    return "Y";
                case nameof(RentalListingRowUntyped.BedroomsQty):
                    return "2";
                case nameof(RentalListingRowUntyped.NightsBookedQty):
                    return "10";
                case nameof(RentalListingRowUntyped.ReservationsQty):
                    return "5";
                case nameof(RentalListingRowUntyped.PropertyHostNm):
                    return "Host Name";
                case nameof(RentalListingRowUntyped.PropertyHostEmail):
                    return "host@example.com";
                case nameof(RentalListingRowUntyped.PropertyHostPhone):
                    return "123-456-7890";
                case nameof(RentalListingRowUntyped.PropertyHostFax):
                    return "123-456-7890";
                case nameof(RentalListingRowUntyped.PropertyHostAddress):
                    return "526 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost1Nm):
                    return "Supplier1 Name";
                case nameof(RentalListingRowUntyped.SupplierHost1Email):
                    return "supplier1@example.com";
                case nameof(RentalListingRowUntyped.SupplierHost1Phone):
                    return "111-222-3333";
                case nameof(RentalListingRowUntyped.SupplierHost1Fax):
                    return "111-222-3333";
                case nameof(RentalListingRowUntyped.SupplierHost1Address):
                    return "527 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost1Id):
                    return "host1";
                case nameof(RentalListingRowUntyped.SupplierHost2Nm):
                    return "Supplier2 Name";
                case nameof(RentalListingRowUntyped.SupplierHost2Email):
                    return "supplier2@example.com";
                case nameof(RentalListingRowUntyped.SupplierHost2Phone):
                    return "222-333-4444";
                case nameof(RentalListingRowUntyped.SupplierHost2Fax):
                    return "222-333-4444";
                case nameof(RentalListingRowUntyped.SupplierHost2Address):
                    return "528 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost2Id):
                    return "host2";
                case nameof(RentalListingRowUntyped.SupplierHost3Nm):
                    return "Supplier3 Name";
                case nameof(RentalListingRowUntyped.SupplierHost3Email):
                    return "supplier3@example.com";
                case nameof(RentalListingRowUntyped.SupplierHost3Phone):
                    return "333-444-5555";
                case nameof(RentalListingRowUntyped.SupplierHost3Fax):
                    return "333-444-5555";
                case nameof(RentalListingRowUntyped.SupplierHost3Address):
                    return "529 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost3Id):
                    return "host3";
                case nameof(RentalListingRowUntyped.SupplierHost4Nm):
                    return "Supplier4 Name";
                case nameof(RentalListingRowUntyped.SupplierHost4Email):
                    return "supplier4@example.com";
                case nameof(RentalListingRowUntyped.SupplierHost4Phone):
                    return "444-555-6666";
                case nameof(RentalListingRowUntyped.SupplierHost4Fax):
                    return "444-555-6666";
                case nameof(RentalListingRowUntyped.SupplierHost4Address):
                    return "530 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost4Id):
                    return "host4";
                case nameof(RentalListingRowUntyped.SupplierHost5Nm):
                    return "Supplier5 Name";
                case nameof(RentalListingRowUntyped.SupplierHost5Email):
                    return "supplier5@example.com";
                case nameof(RentalListingRowUntyped.SupplierHost5Phone):
                    return "555-666-7777";
                case nameof(RentalListingRowUntyped.SupplierHost5Fax):
                    return "555-666-7777";
                case nameof(RentalListingRowUntyped.SupplierHost5Address):
                    return "531 Superior St, Victoria, BC";
                case nameof(RentalListingRowUntyped.SupplierHost5Id):
                    return "host5";
                default:
                    return new NoSpecimen();
            }
        }

    }
}