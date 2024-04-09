export interface DelistingRequest {
    lgId: number;
    platformId: number;
    listingId: string;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
}
