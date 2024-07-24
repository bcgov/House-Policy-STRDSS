export interface User {
    id: number;
    userName: string;
    userGuid: string;
    identityProviderNm: string;
    emailAddress: string;
    firstName: string;
    lastName: string;
    fullName: string;
    displayName: string;
    isActive: boolean;
    businessNm: string;
    accessRequestStatus: string;
    accessRequestRequired: boolean;
    permissions: Array<string>;
    organizationType: string;
    organizationId: number;
    termsAcceptanceDtm: string;
}

export interface UserDetails {
    userIdentityId: number,
    displayNm: string,
    identityProviderNm: string,
    isEnabled: boolean,
    accessRequestStatusCd: string
    accessRequestDtm: string,
    accessGrantedDtm: string,
    accessRequestJustificationTxt: string,
    givenNm: string,
    familyNm: string,
    emailAddressDsc: string,
    businessNm: string,
    termsAcceptanceDtm: string,
    representedByOrganizationId: number,
    updDtm: string,
    representedByOrganization: UserDetailsRepresentedOrganization,
    roleCds: string[],
}

export interface UserDetailsRepresentedOrganization {
    value: number,
    organizationType: string,
    organizationCd: string,
    label: string,
    managingOrganizationId: number,
    updDtm: string,
    updUserGuid: string,
    isLgParticipating: boolean,
    isPrincipalResidenceRequired: boolean,
    isBusinessLicenceRequired: boolean,
    contactPeople: UserDetailsRepresentedOrganizationContact[],
}

export interface UserDetailsRepresentedOrganizationContact {
    organizationContactPersonId: number,
    isPrimary: boolean,
    givenNm: string,
    familyNm: string,
    phoneNo: string,
    emailAddressDsc: string,
    contactedThroughOrganizationId: number,
    emailMessageType: string,
    updDtm: string,
    updUserGuid: string,
}