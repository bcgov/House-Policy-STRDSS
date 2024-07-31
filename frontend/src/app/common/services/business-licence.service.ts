import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BusinessLicenceService {
  constructor(private httpClient: HttpClient) { }

  getUploadHistory(): Observable<any> {
    // TODO: Integrate with a backend
    // return this.httpClient.get('');
    return of([
      {
        status: 'Pending',
        totalRecords: 42,
        uploadDate: '2024-04-09T20:12:49.419678Z',
        uploadBy: 'CEU Admin',
      },
      {
        status: 'Processed',
        totalRecords: 118,
        uploadDate: '2024-04-02T23:11:39.361639Z',
        uploadBy: 'CEU',
      },
      {
        status: 'Processed',
        totalRecords: 10,
        uploadDate: '2024-04-15T20:14:44.1417Z',
        uploadBy: 'Test User',
      },
    ] as Array<{ status: string, totalRecords: number, uploadDate: string, uploadBy: string }>)
  }

  uploadFile(file: any): Observable<any> {
    // TODO: Integrate with a backend
    // return this.httpClient.post('', file);
    return of(file);
  }
}
