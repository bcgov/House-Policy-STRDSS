export interface ListingTableRow {
    rentalListingId: number;
    listingStatusType: string;
    offeringOrganizationNm: string;
    platformListingNo: string;
    originalAddressTxt: string;
    matchAddressTxt: string;
    isEntireUnit: boolean;
    nightsBookedYtdQty: number;
    businessLicenceNo: string;
    businessLicenceNoMatched: string;
    lastActionNm: string;
    lastActionDtm: string;

    offeringOrganizationId?: number;
    listingStatusSortNo?: number;
    latestReportPeriodYm?: string;
    isActive?: boolean;
    isNew?: boolean;
    isTakenDown?: boolean;
    platformListingUrl?: string;
    matchScoreAmt?: number;
    addressSort1ProvinceCd?: string;
    addressSort2LocalityNm?: string;
    addressSort3LocalityTypeDsc?: string;
    addressSort4StreetNm?: string;
    addressSort5StreetTypeDsc?: string;
    addressSort6StreetDirectionDsc?: string;
    addressSort7CivicNo?: string;
    addressSort8UnitNo?: string;
    listingContactNamesTxt?: string;
    managingOrganizationId?: number;
    managingOrganizationNm?: string;
    isChangedBusinessLicence: boolean;
    isPrincipalResidenceRequired?: boolean;
    isBusinessLicenceRequired?: boolean;
    availableBedroomsQty?: number;
    separateReservationsYtdQty?: number
    bcRegistryNo?: string;

    rentalListingContacts?: [];
    selected?: boolean;

}

export interface AggregatedListingTableRow {
    effectiveBusinessLicenceNo?: string;
    effectiveHostNm?: string;
    primaryHostNm: string;
    matchAddressTxt: string;
    nightsBookedYtdQty: number;
    businessLicenceNo: string;
    lastActionNm?: string;
    lastActionDtm?: string;
    businessLicenceExpiryDt?: string;
    businessLicenceId?: number;
    licenceStatusType?: 'EXPIRED' | 'REVOKED' | 'ISSUED' | 'CANCELLED' | 'PENDING' | 'SUSPENDED';
    listingCount: number;
    listings: Array<ListingTableRow>;

    hasMultipleProperties: boolean;
    selected?: boolean;
    id?: string;
}
