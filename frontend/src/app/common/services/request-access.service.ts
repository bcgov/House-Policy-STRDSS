import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DropdownOption } from '../models/dropdown-option';
import { environment } from '../../../environments/environment';
import { AccessRequest } from '../models/access-request';

@Injectable({
  providedIn: 'root'
})
export class RequestAccessService {

  constructor(private httpClient: HttpClient) { }

  getOrganizationTypes(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations/types`);
  }

  createAccessRequest(body: AccessRequest): Observable<unknown> {
    return this.httpClient.post<unknown>(`${environment.API_HOST}/users/accessrequests`, body);
  }
}
