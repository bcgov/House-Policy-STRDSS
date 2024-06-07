using StrDss.Common;
using StrDss.Service;
using Xunit;
using StrDss.Model.RentalReportDtos;

namespace StrDss.Test
{
    public class RentalListingFieldValidatorShould
    {
        [Theory]
        [AutoDomainData]
        public void Validate_ValidDto_ReturnsNoErrors(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_RptPeriod_InvalidFormat_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.RptPeriod = "2024/05"; 

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("RptPeriod", result.Keys);
            Assert.Single(result["RptPeriod"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_RptPeriod_Empty_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.RptPeriod = ""; 

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("RptPeriod", result.Keys);
            Assert.Single(result["RptPeriod"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_OrgCd_ExceedsMaxLength_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.OrgCd = new string('A', 26); // Exceeds max length

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("OrgCd", result.Keys);
            Assert.Single(result["OrgCd"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_ListingUrl_Null_ReturnsNoErrors(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.ListingUrl = null; // Null value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_IsEntireUnit_InvalidValue_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.IsEntireUnit = "X"; // Invalid value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("IsEntireUnit", result.Keys);
            Assert.Single(result["IsEntireUnit"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_BedroomsQty_InvalidValue_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.BedroomsQty = "256x"; // invalid value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("BedroomsQty", result.Keys);
            Assert.Single(result["BedroomsQty"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_NightsBookedQty_InvalidValue_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.NightsBookedQty = "256x"; // invalid value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("NightsBookedQty", result.Keys);
            Assert.Single(result["NightsBookedQty"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_ReservationsQty_InvalidValue_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.ReservationsQty = "256x"; // Exceeds max value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Contains("ReservationsQty", result.Keys);
            Assert.Single(result["ReservationsQty"]);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_SupplierHost2Nm_Null_ReturnsNoErrors(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.SupplierHost2Nm = null; // Null value

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [AutoDomainData]
        public void Validate_SupplierHost2Id_ExceedsMaxLength_ReturnsError(
            RentalListingRowUntyped dto,
            FieldValidatorService sut)
        {
            // Arrange
            dto.SupplierHost2Id = new string('A', 26); // Exceeds max length

            var errors = new Dictionary<string, List<string>>();

            // Act
            var result = sut.Validate(Entities.RentalListingRowUntyped, dto, errors);

            Assert.Contains("SupplierHost2Id", result.Keys);
            Assert.Single(result["SupplierHost2Id"]);
        }
    }

}
