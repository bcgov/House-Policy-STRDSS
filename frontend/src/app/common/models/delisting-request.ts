export interface DelistingRequest {
    lgId: number;
    platformId: number;
    listingId: number;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
}
