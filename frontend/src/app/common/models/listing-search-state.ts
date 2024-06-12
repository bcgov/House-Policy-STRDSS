export interface ListingSearchState {
    searchTerm?: string;
    searchBy?: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicense';
    sortColumn?: string;
    sortDirection?: 'asc' | 'desc';
    pageNumber?: number;
    pageSize?: number;
} 