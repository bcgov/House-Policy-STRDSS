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