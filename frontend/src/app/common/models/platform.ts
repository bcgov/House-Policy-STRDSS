export interface Platform {
    id?: string;
    organizationId: number;
    organizationType: string;
    organizationCd: string;
    organizationNm: string;
    updDtm: string;
    isActive: boolean;
    updUserGuid: string;
    primaryNoticeOfTakedownContactId: number;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactId: number;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactId: number;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactId: number;
    secondaryTakedownRequestContactEmail: string;
    subsidiaries: Array<SubPlatform>;
    platformType: string;
    updUserDisplayNm: string;
}

export interface UpdatePlatform {
    organizationNm: string;
    updDtm: string;
    isActive: boolean;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactEmail: string;
    platformType: string;
}

export interface UpdateSubPlatform {
    organizationNm: string;
    updDtm: string;
    isActive: boolean;
    managingOrganizationId: number;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactEmail: string;
}

export interface SubPlatform {
    id?: string;
    organizationId: number;
    organizationType: string;
    organizationCd: string;
    organizationNm: string;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactEmail: string;
    updDtm: string;
    isActive: boolean;
    updUserGuid: string;
    managingOrganizationId: number;
    updUserDisplayNm: string;
}

export interface PlatformCreate {
    organizationCd: string;
    organizationNm: string;
    isActive: boolean;
    platformType: string;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactEmail: string;
}

export interface SubPlatformCreate {
    organizationCd: string;
    organizationNm: string;
    managingOrganizationId: number;
    isActive: boolean;
    updDtm: string;
    primaryNoticeOfTakedownContactEmail: string;
    primaryTakedownRequestContactEmail: string;
    secondaryNoticeOfTakedownContactEmail: string;
    secondaryTakedownRequestContactEmail: string;
}
