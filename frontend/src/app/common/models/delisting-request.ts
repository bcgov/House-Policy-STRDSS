export interface DelistingRequest {
    lgId: number;
    platformId: string;
    listingId: number;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
}
