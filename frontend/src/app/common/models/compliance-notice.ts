export interface ComplianceNotice {
    platformId: number;
    listingId?: string;
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

export interface ComplianceNoticeBulk {
    rentalListingId: number;
    hostEmailSent: boolean;
    ccList: Array<string>;
    comment: string;
    lgContactEmail: string;
}
