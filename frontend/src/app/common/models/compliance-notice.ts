export interface ComplianceNotice {
    platformId: number;
    listingUrl: string;
    hostEmail: string;
    reasonId: number;
    sendCopy: boolean;
    ccList: Array<string>;
    comment: string;
}
