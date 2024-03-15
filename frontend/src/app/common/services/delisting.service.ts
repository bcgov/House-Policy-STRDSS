import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption } from '../models/dropdown-option';
import { ComplianceNotice } from '../models/compliance-notice';

@Injectable({
  providedIn: 'root'
})
export class DelistingService {
  textHeaders = new HttpHeaders().set('Content-Type', 'application/json');

  constructor(private httpClient: HttpClient) { }

  getPlatforms(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/platforms/dropdown`)
  }

  getReasons(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/delisting/reasons/dropdown`)
  }

  complianceNoticePreview(complianceNotice: ComplianceNotice): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/warnings/preview`, complianceNotice)
  }

  createComplianceNotice(complianceNotice: ComplianceNotice): Observable<void> {
    return this.httpClient.post<void>(`${environment.API_HOST}/delisting/warnings`, complianceNotice)
  }

  delistingRequestPreview(delistingRequest: ComplianceNotice): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/requests/preview`, delistingRequest)
  }

  createDelistingRequest(delistingRequest: ComplianceNotice): Observable<void> {
    return this.httpClient.post<void>(`${environment.API_HOST}/delisting/requests`, delistingRequest)
  }
}
