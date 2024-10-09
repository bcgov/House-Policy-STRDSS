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
    subsidiaries: Array<Platform>;
    platformType: string;
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