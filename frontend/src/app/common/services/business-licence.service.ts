import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BusinessLicenceService {
  textHeaders = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');

  constructor(private httpClient: HttpClient) { }

  getUploadHistory(
    pageNumber: number = 1,
    pageSize: number = 10,
    orderBy: string = '',
    direction: 'asc' | 'desc' = 'asc',
    orgId: number = 0,
  ): Observable<any> {
    let url = `${environment.API_HOST}/bizlicences/uploadhistory?pageSize=${pageSize}&pageNumber=${pageNumber}`;

    if (!!orgId) {
      url += `&orgId=${orgId}`;
    }

    if (orderBy) {
      url += `&orderBy=${orderBy}&direction=${direction}`;
    }

    return this.httpClient.get<any>(url);
  }

  downloadErrors(id: number): Observable<any> {
    return this.httpClient.get<any>(`${environment.API_HOST}/rentallistingreports/uploads/${id}/errorfile`,
      { headers: this.textHeaders, responseType: 'text' as 'json' });
  }

  uploadFile(file: any, orgId: number): Observable<any> {
    const formData = new FormData();
    formData.append('organizationId', orgId.toString());
    formData.append('file', file);

    return this.httpClient.post<any>(
      `${environment.API_HOST}/bizlicences`,
      formData,
    );
  }
}
