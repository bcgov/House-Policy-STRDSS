export interface ListingTableRow {
    id: number;
    status: string;
    platformName: string;
    platformId: string;
    listingId: string;
    addressRaw: string;
    addressNormalized: string;
    entireUnit: boolean;
    nightsStayed: number;
    license: string;
    lastAction: string;
    lastActionDate: string;
}