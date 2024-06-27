import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption } from '../models/dropdown-option';
import { ComplianceNotice, ComplianceNoticeBulk } from '../models/compliance-notice';
import { DelistingRequest, DelistingRequestBulk } from '../models/delisting-request';

@Injectable({
  providedIn: 'root'
})
export class DelistingService {
  textHeaders = new HttpHeaders().set('Content-Type', 'application/json');

  constructor(private httpClient: HttpClient) { }

  getPlatforms(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations/dropdown/?type=Platform`)
  }

  getLocalGovernments(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations?type=LG`)
  }

  getReasons(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/delisting/reasons/dropdown`)
  }

  complianceNoticePreview(complianceNotice: ComplianceNotice): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/warnings/preview`, complianceNotice)
  }

  createComplianceNotice(complianceNotice: ComplianceNotice): Observable<any> {
    return this.httpClient.post<any>(`${environment.API_HOST}/delisting/warnings`, complianceNotice)
  }

  delistingRequestPreview(delistingRequest: DelistingRequest): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/requests/preview`, delistingRequest)
  }

  createDelistingRequest(delistingRequest: DelistingRequest): Observable<any> {
    return this.httpClient.post<any>(`${environment.API_HOST}/delisting/requests`, delistingRequest)
  }

  delistingRequestBulkPreview(delistingRequests: Array<DelistingRequestBulk>): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/bulkrequests/preview`, delistingRequests)
  }

  delistingRequestBulk(delistingRequests: Array<DelistingRequestBulk>): Observable<void> {
    return this.httpClient.post<void>(`${environment.API_HOST}/delisting/bulkrequests`, delistingRequests)
  }

  complianceNoticeBulkPreview(delistingRequests: Array<ComplianceNoticeBulk>): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/bulkwarnings/preview`, delistingRequests)
  }

  complianceNoticeBulk(delistingRequests: Array<ComplianceNoticeBulk>): Observable<void> {
    return this.httpClient.post<void>(`${environment.API_HOST}/delisting/bulkwarnings`, delistingRequests)
  }
}
