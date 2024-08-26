export interface BusinessLicence {

    businessLicenceId: number;

    updDtm: string;

    businessNm?: string;
    physicalRentalAddressTxt: string;

    businessLicenceNo: string;
    licenceTypeTxt: string;
    expiryDt: string;
    licenceStatusType: 'ISSUED' | 'EXPIRED' | 'SUSPENDED' | 'REVOKED' | 'CANCELLED' | 'PENDING';
    restrictionTxt?: string;

    businessOwnerNm?: string;
    businessOwnerEmailAddressDsc?: string;
    businessOwnerPhoneNo?: string;

    businessOperatorNm?: string;
    businessOperatorEmailAddressDsc?: string;
    businessOperatorPhoneNo?: string;

    propertyZoneTxt?: string;
    availableBedroomsQty: number;
    maxGuestsAllowedQty: number;
    isPrincipalResidence: boolean;
    isOwnerLivingOnsite: boolean;
    isOwnerPropertyTenant: boolean;
    infractionTxt?: string;
    infractionDt?: string;

    propertyFolioNo?: string;
    propertyParcelIdentifierNo?: string;
    propertyLegalDescriptionTxt?: string;

    providingOrganizationId: number;
    affectedByPhysicalAddressId: number;
    updUserGuid: string;
    licenceStatus: {
        licenceStatusType: "ISSUED";
        licenceStatusTypeNm: "Issued";
        licenceStatusSortNo: number;
    }

    mailingStreetAddressTxt: string;
    mailingCityNm: string;
    mailingProvinceCd: string;
    mailingPostalCd: string;
}