export interface ComplianceNotice {
    platformId: number;
    listingId?: number;
    listingUrl: string;
    hostEmail?: string;
    hostEmailSent: boolean;
    reasonId: number;
    sendCopy: boolean;
    ccList: Array<string>;
    LgContactEmail: string;
    LgContactPhone?: string;
    StrBylawUrl?: string;
    comment: string;
}
