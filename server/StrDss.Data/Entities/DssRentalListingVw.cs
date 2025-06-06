﻿using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssRentalListingVw
{
    public long? RentalListingId { get; set; }

    public string? ListingStatusType { get; set; }

    public string? ListingStatusTypeNm { get; set; }

    public short? ListingStatusSortNo { get; set; }

    public DateOnly? LatestReportPeriodYm { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsTakenDown { get; set; }

    public bool? IsLgTransferred { get; set; }

    public bool? IsChangedAddress { get; set; }

    public long? OfferingOrganizationId { get; set; }

    public string? OfferingOrganizationCd { get; set; }

    public string? OfferingOrganizationNm { get; set; }

    public string? PlatformListingNo { get; set; }

    public string? PlatformListingUrl { get; set; }

    public string? OriginalAddressTxt { get; set; }

    public bool? IsMatchCorrected { get; set; }

    public bool? IsMatchVerified { get; set; }

    public short? MatchScoreAmt { get; set; }

    public string? MatchAddressTxt { get; set; }

    public string? AddressSort1ProvinceCd { get; set; }

    public string? AddressSort2LocalityNm { get; set; }

    public string? AddressSort3LocalityTypeDsc { get; set; }

    public string? AddressSort4StreetNm { get; set; }

    public string? AddressSort5StreetTypeDsc { get; set; }

    public string? AddressSort6StreetDirectionDsc { get; set; }

    public string? AddressSort7CivicNo { get; set; }

    public string? AddressSort8UnitNo { get; set; }

    public string? ListingContactNamesTxt { get; set; }

    public long? ManagingOrganizationId { get; set; }

    public string? ManagingOrganizationNm { get; set; }

    public string? EconomicRegionDsc { get; set; }

    public bool? IsPrincipalResidenceRequired { get; set; }

    public bool? IsBusinessLicenceRequired { get; set; }

    public bool? IsEntireUnit { get; set; }

    public short? AvailableBedroomsQty { get; set; }

    public short? NightsBookedYtdQty { get; set; }

    public short? SeparateReservationsYtdQty { get; set; }

    public string? BusinessLicenceNo { get; set; }

    public string? BcRegistryNo { get; set; }

    public string? LastActionNm { get; set; }

    public DateTime? LastActionDtm { get; set; }

    public long? BusinessLicenceId { get; set; }

    public string? BusinessLicenceNoMatched { get; set; }

    public DateOnly? BusinessLicenceExpiryDt { get; set; }

    public string? LicenceStatusType { get; set; }

    public string? EffectiveBusinessLicenceNo { get; set; }

    public string? EffectiveHostNm { get; set; }

    public bool? IsChangedBusinessLicence { get; set; }

    public DateTime? LgTransferDtm { get; set; }

    public string? TakeDownReason { get; set; }
}
