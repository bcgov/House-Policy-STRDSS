export interface DelistingRequest {
    platformId: number;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
    comment: string;
}
