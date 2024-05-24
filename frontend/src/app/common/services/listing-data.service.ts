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

  getListings(
    pageNumber: number = 1,
    pageSize: number = 10,
    orderBy: string = '',
    direction: 'asc' | 'desc' = 'asc'
  ): Observable<PagingResponse<ListingTableRow>> {
    let url = `${environment.API_HOST}/rentallistings?pageSize=${pageSize}&pageNumber=${pageNumber}`;

    if (orderBy) {
      url += `&orderBy=${orderBy}&direction=${direction}`;
    }

    return this.httpClient.get<PagingResponse<ListingTableRow>>(url);
  }
}
