export interface ListingSearchState {
    searchTerm?: string;
    searchBy?: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicence';
    sortColumn?: string;
    sortDirection?: 'asc' | 'desc';
    pageNumber?: number;
    pageSize?: number;
} 