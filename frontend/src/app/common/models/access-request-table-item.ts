export interface AccessRequestTableItem {
    userIdentityId: number;
    isEnabled: boolean;
    accessRequestStatusCd: string;
    accessRequestDtm: string;
    accessRequestJustificationTxt: string;
    givenNm: string;
    familyNm: string;
    emailAddressDsc: string;
    businessNm: string;
    termsAcceptanceDtm: string;
    representedByOrganizationId: number;
    organizationType: string;
    organizationCd: string;
    organizationNm: string;
    updDtm: string;
}
