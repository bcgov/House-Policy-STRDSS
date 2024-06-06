export interface ListingDetails {
    rentalListingId: number;
    listingStatusType: 'N' | 'I' | 'R' | 'A';
    listingStatusSortNo: number;
    latestReportPeriodYm: string;
    isActive: boolean;
    isNew: boolean;
    isTakenDown: boolean;
    offeringOrganizationId: number;
    offeringOrganizationNm: string;
    platformListingNo: number;
    platformListingUrl: string;
    originalAddressTxt: string;
    matchScoreAmt: number;
    matchAddressTxt: string;
    addressSort1ProvinceCd: string;
    addressSort2LocalityNm: string;
    addressSort3LocalityTypeDsc: string;
    addressSort4StreetNm: string;
    addressSort5StreetTypeDsc: string;
    addressSort6StreetDirectionDsc: string;
    addressSort7CivicNo: string;
    addressSort8UnitNo: string;
    listingContactNamesTxt: string;
    managingOrganizationId: number;
    managingOrganizationNm: string;
    isPrincipalResidenceRequired: boolean;
    isBusinessLicenceRequired: boolean;
    isEntireUnit: boolean;
    availableBedroomsQty: number;
    nightsBookedYtdQty: number;
    separateReservationsYtdQty: number;
    businessLicenceNo: number;
    bcRegistryNo: number;
    lastActionNm: string;
    lastActionDtm: string;
    hosts: Array<ListingDetailsHost>;
    listingHistory: Array<ListingDetailsListingHistory>;
    actionHistory: Array<ListingDetailsActionHistory>;
}

export interface ListingDetailsHost {
    rentalListingContactId: number;
    isPropertyOwner: boolean;
    listingContactNbr: number;
    supplierHostNo: string;
    fullNm: string;
    phoneNo: string;
    faxNo: string;
    fullAddressTxt: string;
    emailAddressDsc: string;
    contactedThroughRentalListingId: number;
    updDtm: string;
    updUserGuid: string;
}

export interface ListingDetailsListingHistory {
    reportPeriodYM: string,
    nightsBookedQty: number,
    separateReservationsQty: number
}

export interface ListingDetailsActionHistory {
    [key: string]: any;
}