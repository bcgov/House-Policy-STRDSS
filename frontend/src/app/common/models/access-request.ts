export interface AccessRequest {
    organizationType: string,
    organizationName: string,
}

export interface ApproveRequestModel {
    userIdentityId: number;
    representedByOrganizationId: number;
    isEnabled: boolean;
    updDtm: string;
}

export interface DenyRequestModel {
    userIdentityId: number;
    updDtm: string;
}