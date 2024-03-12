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
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/compliancenotices/reasons/dropdown`)
  }

  complianceNoticePreview(complianceNotice: ComplianceNotice): Observable<string> {
    return this.httpClient.post<string>(`${environment.API_HOST}/compliancenotices/preview`, complianceNotice, {
      headers: this.textHeaders, responseType: 'text' as any
    })
  }

  createComplianceNotice(complianceNotice: ComplianceNotice): Observable<string> {
    return this.httpClient.post<string>(`${environment.API_HOST}/compliancenotices`, complianceNotice)
  }

  delistingRequestPreview(delistingRequest: ComplianceNotice): Observable<string> {
    return this.httpClient.post<string>(`${environment.API_HOST}/delisting/requests/preview`, delistingRequest, {
      headers: this.textHeaders, responseType: 'text' as any
    })
  }

  createDelistingRequest(delistingRequest: ComplianceNotice): Observable<string> {
    return this.httpClient.post<string>(`${environment.API_HOST}/delisting/requests`, delistingRequest)
  }
}
