export interface ComplianceNotice {
    platformId: string;
    listingId?: number;
    listingUrl: string;
    hostEmail?: string;
    sentAlternatively: boolean;
    reasonId: number;
    sendCopy: boolean;
    ccList: Array<string>;
    LgContactEmail: string;
    LgContactPhone?: string;
    StrBylawUrl?: string;
    comment: string;
}
