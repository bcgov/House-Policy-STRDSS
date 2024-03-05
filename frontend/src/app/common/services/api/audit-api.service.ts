import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HistoryRow } from '../../models/history-row.model';

@Injectable({
  providedIn: 'root'
})
export class AuditApiService {
  private readonly apiUrl = `api`;

  constructor(private httpClient: HttpClient) {
  }

  getStrApplicationsHistory(id: number): Observable<Array<HistoryRow>> {
    return this.httpClient.get<Array<HistoryRow>>(`${this.apiUrl}/audits/StrApplication/${id}`)
  }
}
