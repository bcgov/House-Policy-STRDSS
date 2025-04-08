import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {
  httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'multipart/form-data',
    }),
  };

  textHeaders = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');

  constructor(private httpClient: HttpClient) { }

  uploadRegistrations(organizationId: number, file: any): Observable<unknown> {
    const formData = new FormData();
    formData.append('organizationId', organizationId.toString());
    formData.append('file', file);

    return this.httpClient.post<any>(`${environment.API_HOST}/registration`, formData);
  }

  getRegistrationValidationHistory(
    pageNumber: number = 1,
    pageSize: number = 10,
    orderBy: string = '',
    direction: 'asc' | 'desc' = 'asc',
    platformId: number = 0,
  ): Observable<any> {
    let url = `${environment.API_HOST}/registration/registrationvalidationhistory?pageSize=${pageSize}&pageNumber=${pageNumber}`;

    if (!!platformId) {
      url += `&platformId=${platformId}`;
    }

    if (orderBy) {
      url += `&orderBy=${orderBy}&direction=${direction}`;
    }

    return this.httpClient.get<any>(url);
  }
}
