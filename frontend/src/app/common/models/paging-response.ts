export interface PagingResponse<T> {
    sourceList: Array<T>;
    pageInfo: PagingResponsePageInfo
}

export interface PagingResponsePageInfo {
    pageNumber: number,
    pageSize: number,
    itemCount: number,
    totalCount: number,
    pageCount: number,
    hasPreviousPage: boolean,
    hasNextPage: boolean,
    orderBy: string,
    direction: string
}