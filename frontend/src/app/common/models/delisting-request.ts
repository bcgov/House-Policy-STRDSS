export interface DelistingRequest {
    platformId: number;
    listingId: string;
    listingUrl: string;
    sendCopy: boolean;
    ccList: Array<string>;
    isWithStandardDetail: boolean;
    customDetailTxt: string;
}

export interface DelistingRequestBulk {
    rentalListingId: number;
    ccList: Array<string>;
    isWithStandardDetail: boolean;
    customDetailTxt: string;
}