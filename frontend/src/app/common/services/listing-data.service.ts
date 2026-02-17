import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagingResponse } from '../models/paging-response';
import { ListingUploadHistoryRecord } from '../models/listing-upload-history-record';
import { ListingTableRow } from '../models/listing-table-row';
import { ListingSearchRequest } from '../models/listing-search-request';
import { ListingAddressCandidate, ListingDetails } from '../models/listing-details';
import { ExportJurisdiction } from '../models/export-listing';
import { ListingFilter } from '../models/listing-filter';
import { ListingResponseWithCounts, AggregatedListingResponseWithCounts } from '../models/listing-response-with-counts';

@Injectable({
    providedIn: 'root',
})
export class ListingDataService {
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'multipart/form-data',
        }),
    };

    textHeaders = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');

    constructor(private httpClient: HttpClient) { }

    uploadListings(reportPeriod: string, organizationId: number, file: any): Observable<unknown> {
        const formData = new FormData();
        formData.append('reportPeriod', reportPeriod);
        formData.append('organizationId', organizationId.toString());
        formData.append('file', file);

        return this.httpClient.post<any>(`${environment.API_HOST}/RentalListingReports`, formData);
    }

    uploadTakedown(reportPeriod: string, organizationId: number, file: any): Observable<unknown> {
        const formData = new FormData();
        formData.append('reportPeriod', reportPeriod);
        formData.append('organizationId', organizationId.toString());
        formData.append('file', file);

        return this.httpClient.post<any>(
            `${environment.API_HOST}/RentalListings/takedownconfirmation`,
            formData,
        );
    }

    getListingUploadHistoryRecords(
        pageNumber: number = 1,
        pageSize: number = 10,
        platformId: number = 0,
        orderBy: string = '',
        direction: 'asc' | 'desc' = 'asc',
    ): Observable<PagingResponse<ListingUploadHistoryRecord>> {
        let url = `${environment.API_HOST}/rentallistingreports/rentallistinghistory?pageSize=${pageSize}&pageNumber=${pageNumber}`;

        if (!!platformId) {
            url += `&platformId=${platformId}`;
        }

        if (orderBy) {
            url += `&orderBy=${orderBy}&direction=${direction}`;
        }

        return this.httpClient.get<PagingResponse<ListingUploadHistoryRecord>>(url);
    }

    getUploadHistoryErrors(id: number): Observable<any> {
        return this.httpClient.get<any>(
            `${environment.API_HOST}/rentallistingreports/uploads/${id}/errorfile`,
            { headers: this.textHeaders, responseType: 'text' as 'json' },
        );
    }

    getListings(
        pageNumber: number = 1,
        pageSize: number = 10,
        orderBy: string = '',
        direction: 'asc' | 'desc' = 'asc',
        searchReq: ListingSearchRequest = {},
        filter?: ListingFilter,
        recent: boolean = false,
    ): Observable<ListingResponseWithCounts<ListingTableRow>> {
        let endpointUrl = `${environment.API_HOST}/rentallistings?pageSize=${pageSize}&pageNumber=${pageNumber}`;

        if (orderBy) {
            endpointUrl += `&orderBy=${orderBy}&direction=${direction}`;
        }

        if (recent) {
            endpointUrl += `&recent=${recent}`;
        }

        if (searchReq.all) {
            endpointUrl += `&all=${searchReq.all}`;
        }
        if (searchReq.address) {
            endpointUrl += `&address=${searchReq.address}`;
        }
        if (searchReq.url) {
            endpointUrl += `&url=${searchReq.url}`;
        }
        if (searchReq.listingId) {
            endpointUrl += `&listingId=${searchReq.listingId}`;
        }
        if (searchReq.hostName) {
            endpointUrl += `&hostName=${searchReq.hostName}`;
        }
        if (searchReq.businessLicence) {
            endpointUrl += `&businessLicence=${searchReq.businessLicence}`;
        }
        if (searchReq.registrationNumber) {
            endpointUrl += `&registrationNumber=${searchReq.registrationNumber}`;
        }

        if (filter) {
            if (filter.byLocation) {
                if (!!filter.byLocation?.isPrincipalResidenceRequired) {
                    endpointUrl += `&prRequirement=${filter.byLocation.isPrincipalResidenceRequired == 'Yes'
                        }`;
                }
                if (!!filter.byLocation?.isBusinessLicenceRequired) {
                    endpointUrl += `&blRequirement=${filter.byLocation.isBusinessLicenceRequired == 'Yes'
                        }`;
                }
            }
            if (filter.byStatus) {
                if (
                    filter.byStatus.reassigned !== null &&
                    filter.byStatus.reassigned !== undefined
                ) {
                    endpointUrl += `&reassigned=${!!filter.byStatus.reassigned}`;
                }
                if (
                    filter.byStatus.takedownComplete !== null &&
                    filter.byStatus.takedownComplete !== undefined
                ) {
                    endpointUrl += `&takedownComplete=${!!filter.byStatus.takedownComplete}`;
                }

                const statuses = new Array();
                if (filter.byStatus.active) statuses.push('A');
                if (filter.byStatus.inactive) statuses.push('I');
                if (filter.byStatus.new) statuses.push('N');

                if (statuses.length) {
                    endpointUrl += `&statuses=${statuses.join(',')}`;
                }
            }
            if (!!filter.community) {
                endpointUrl += `&lgId=${filter.community}`;
            }
        }

        return this.httpClient.get<ListingResponseWithCounts<ListingTableRow>>(endpointUrl);
    }

    getHostListingsCount(primaryHostNm: string): Observable<{ primaryHostNm: string, hasMultipleProperties: boolean }> {
        return this.httpClient.get<number>(`${environment.API_HOST}/rentallistings/host/count?hostName=${primaryHostNm}`)
            .pipe(map(count => ({ primaryHostNm, hasMultipleProperties: count > 1 })));
    }

    getAggregatedListings(
        searchReq: ListingSearchRequest = {},
        filter?: ListingFilter,
        recent: boolean = false,
    ): Observable<AggregatedListingResponseWithCounts> {
        let listingsEndpointUrl = `${environment.API_HOST}/rentallistings/grouped`;
        const params: string[] = [];

        if (recent) {
            params.push(`recent=${recent}`);
        }

        if (searchReq.all) {
            params.push(`all=${searchReq.all}`);
        }
        if (searchReq.address) {
            params.push(`address=${searchReq.address}`);
        }
        if (searchReq.url) {
            params.push(`url=${searchReq.url}`);
        }
        if (searchReq.listingId) {
            params.push(`listingId=${searchReq.listingId}`);
        }
        if (searchReq.hostName) {
            params.push(`hostName=${searchReq.hostName}`);
        }
        if (searchReq.businessLicence) {
            params.push(`businessLicence=${searchReq.businessLicence}`);
        }
        if (searchReq.registrationNumber) {
            params.push(`registrationNumber=${searchReq.registrationNumber}`);
        }

        if (filter) {
            if (filter.byLocation) {
                if (!!filter.byLocation?.isPrincipalResidenceRequired) {
                    params.push(`prRequirement=${filter.byLocation.isPrincipalResidenceRequired == 'Yes'}`);
                }
                if (!!filter.byLocation?.isBusinessLicenceRequired) {
                    params.push(`blRequirement=${filter.byLocation.isBusinessLicenceRequired == 'Yes'}`);
                }
            }
            if (filter.byStatus) {
                if (
                    filter.byStatus.reassigned !== null &&
                    filter.byStatus.reassigned !== undefined
                ) {
                    params.push(`reassigned=${!!filter.byStatus.reassigned}`);
                }
                if (
                    filter.byStatus.takedownComplete !== null &&
                    filter.byStatus.takedownComplete !== undefined
                ) {
                    params.push(`takedownComplete=${!!filter.byStatus.takedownComplete}`);
                }

                const statuses = new Array();
                if (filter.byStatus.active) statuses.push('A');
                if (filter.byStatus.inactive) statuses.push('I');
                if (filter.byStatus.new) statuses.push('N');

                if (statuses.length) {
                    params.push(`statuses=${statuses.join(',')}`);
                }
            }
            if (!!filter.community) {
                params.push(`lgId=${filter.community}`);
            }
        }

        if (params.length > 0) {
            listingsEndpointUrl += `?${params.join('&')}`;
        }

        // API now returns object with data, recentCount, and allCount
        return this.httpClient.get<AggregatedListingResponseWithCounts>(listingsEndpointUrl);
    }

    getListingDetailsById(id: number): Observable<ListingDetails> {
        return this.httpClient.get<ListingDetails>(`${environment.API_HOST}/rentallistings/${id}`);
    }

    getJurisdictions(): Observable<Array<ExportJurisdiction>> {
        return this.httpClient.get<Array<ExportJurisdiction>>(
            `${environment.API_HOST}/rentallistings/exports`,
        );
    }

    downloadListings(jurisdictionId: number): Observable<any> {
        return this.httpClient.get(
            `${environment.API_HOST}/rentallistings/exports/${jurisdictionId}`,
            { responseType: 'arraybuffer' },
        );
    }

    getAddressCandidates(addressString: string): Observable<Array<ListingAddressCandidate>> {
        return this.httpClient.get<Array<ListingAddressCandidate>>(
            `${environment.API_HOST}/rentallistings/addresses/candidates`,
            {
                params: {
                    addressString,
                },
            },
        );
    }

    changeAddress(id: number, addressString: string): Observable<any> {
        return this.httpClient.put<any>(`${environment.API_HOST}/rentallistings/${id}/address`, {
            rentalListingId: id,
            addressString,
        });
    }

    confirmAddress(id: number): Observable<any> {
        return this.httpClient.put<any>(
            `${environment.API_HOST}/rentallistings/${id}/address/confirm`,
            {},
        );
    }
}
