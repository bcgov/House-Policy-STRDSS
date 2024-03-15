export interface DelistingRequest {
    municipalityId: number;
    platformId: number;
    listingId: string;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
    comment: string;
}
