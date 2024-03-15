export interface ComplianceNotice {
    platformId: number;
    listingId?: string;
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
