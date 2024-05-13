import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagingRequest } from '../models/paging-request';
import { PagingResponse } from '../models/paging-response';
import { ListingUploadHistoryRecord } from '../models/listing-upload-history-record';

@Injectable({
  providedIn: 'root'
})
export class ListingDataService {
  httpOptions = {
    headers: new HttpHeaders({
      "Content-Type": "multipart/form-data",
    })
  };
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

  getListingUploadHistoryRecords(paging?: PagingRequest): Observable<PagingResponse<ListingUploadHistoryRecord>> {
    // return this.httpClient.get<PagingResponse<Listing>>(`${environment.API_HOST}/RentalListingReports`);
    // Note: MOCK DATA. DELETE after integration
    const totalMockedItems = 25;
    const perPageMockedItems = 10;
    const mockResult: PagingResponse<ListingUploadHistoryRecord> = {
      pageInfo: {
        pageNumber: 1,
        itemCount: perPageMockedItems,
        pageSize: perPageMockedItems,
        direction: 'asc',
        pageCount: 3,
        totalCount: totalMockedItems,
        hasNextPage: true,
        hasPreviousPage: false,
        orderBy: '',
      },
      sourceList: this.generateRandomListings(perPageMockedItems)
    }

    return of(mockResult);
  }

  // Note: MOCK DATA. DELETE after integration
  private generateRandomListings(numberOfItems: number): ListingUploadHistoryRecord[] {
    const items: ListingUploadHistoryRecord[] = [];

    for (let i = 0; i < numberOfItems; i++) {
      const item: ListingUploadHistoryRecord = {
        id: i + 1,
        platform: `test${Math.floor(Math.random() * 1000) + 1}`,
        reportedDate: this.getRandomDate(-2),
        status: Math.random() < 0.5 ? 'Pending' : 'Processed',
        totalErrors: Math.floor(Math.random() * 10),
        totalRecords: Math.floor(Math.random() * 100),
        totalSuccess: Math.floor(Math.random() * (Math.floor(Math.random() * 1000) + 1)),
        uploadedBy: `Test User${Math.floor(Math.random() * 1000) + 1}`,
        uploadedDate: new Date().toISOString().split('T')[0],
      };

      if (i === 0) {
        item.totalSuccess = -1;
      }

      if (i === 1) {
        item.totalErrors = -1;
      }

      if (i === 2) {
        item.totalRecords = -1;
      }

      items.push(item);
    }

    return items;
  }

  // Note: MOCK DATA. DELETE after integration
  private getRandomDate(monthOffset: number = -2): string {
    const today = new Date();
    today.setMonth(today.getMonth() + monthOffset);
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    return `${year}-${month}`;
  }
}
