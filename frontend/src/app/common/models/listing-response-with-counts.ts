import { PagingResponse } from './paging-response';
import { AggregatedListingTableRow } from './listing-table-row';

export interface ListingResponseWithCounts<T> extends PagingResponse<T> {
    recentCount: number;
    allCount: number;
}

export interface AggregatedListingResponseWithCounts {
    data: AggregatedListingTableRow[];
    recentCount: number;
    allCount: number;
}

