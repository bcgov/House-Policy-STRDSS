import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ComplianceOrder } from '../models/compliance-order';

@Injectable({
  providedIn: 'root'
})
export class ComplianceService {
  constructor(private httpClient: HttpClient) { }

  sendComplianceOrdersPreview(complianceOrder: Array<ComplianceOrder>): Observable<{ content: string }> {
    return this.httpClient.post<{ content: string }>(`${environment.API_HOST}/delisting/complianceorders/preview`, complianceOrder);
  }
  sendComplianceOrdersConfirm(complianceOrder: Array<ComplianceOrder>): Observable<unknown> {
    return this.httpClient.post<unknown>(`${environment.API_HOST}/delisting/complianceorders`, complianceOrder);
  }
}
