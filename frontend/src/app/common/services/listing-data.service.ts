import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagingResponse } from '../models/paging-response';
import { ListingUploadHistoryRecord } from '../models/listing-upload-history-record';
import { ListingTableRow } from '../models/listing-table-row';

@Injectable({
  providedIn: 'root'
})
export class ListingDataService {
  httpOptions = {
    headers: new HttpHeaders({
      "Content-Type": "multipart/form-data",
    })
  };

  textHeaders = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');

  constructor(private httpClient: HttpClient) { }

  uploadData(reportPeriod: string, organizationId: number, file: any): Observable<unknown> {
    const formData = new FormData();
    formData.append('reportPeriod', reportPeriod);
    formData.append('organizationId', organizationId.toString());
    formData.append('file', file);

    return this.httpClient.post<any>(
      `${environment.API_HOST}/RentalListingReports`,
      formData
    );
  }

  getListingUploadHistoryRecords(
    pageNumber: number = 1,
    pageSize: number = 10,
    platformId: number = 0,
    orderBy: string = '',
    direction: 'asc' | 'desc' = 'asc'
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
    return this.httpClient.get<any>(`${environment.API_HOST}/rentallistingreports/uploads/${id}/errorfile`, { headers: this.textHeaders, responseType: 'text' as 'json' });
  }

  getListings(): Observable<PagingResponse<ListingTableRow>> {
    // return this.httpClient.get<any>('url');
    return of({
      pageInfo:
        { itemCount: 10, direction: 'asc', hasNextPage: true, hasPreviousPage: false, orderBy: '', pageCount: 2, pageNumber: 1, pageSize: 10, totalCount: 12 },
      sourceList: this.generateListingTableData(100),
    } as PagingResponse<ListingTableRow>);
  }

  // NOTE: MOCK

  generateMockListingTableRow(id: number): ListingTableRow {
    const statuses = ['active', 'inactive', 'removed', 'new'];
    const platformNames = ['Airbnb', 'Booking.com', 'VRBO'];
    const actions = ['created', 'updated', 'deleted'];

    return {
      id: id,
      status: statuses[Math.floor(Math.random() * statuses.length)],
      platformName: platformNames[Math.floor(Math.random() * platformNames.length)],
      platformId: `PLT${id}`,
      listingId: `LST9385476${id}`,
      addressRaw: `123 Main St, City ${id}`,
      addressNormalized: `123 Main Street, City ${id}, State, ZIP${id}`,
      entireUnit: Math.random() < 0.5,
      nightsStayed: Math.floor(Math.random() * 100),
      license: `34563456${id}`,
      lastAction: actions[Math.floor(Math.random() * actions.length)],
      lastActionDate: new Date().toISOString()
    };
  }

  generateListingTableData(count: number): ListingTableRow[] {
    const listings: ListingTableRow[] = [];
    for (let i = 1; i <= count; i++) {
      listings.push(this.generateMockListingTableRow(i));
    }
    return listings;
  }

}
