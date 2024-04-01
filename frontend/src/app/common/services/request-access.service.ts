import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DropdownOption } from '../models/dropdown-option';
import { environment } from '../../../environments/environment';
import { AccessRequest, ApproveRequestModel, DenyRequestModel } from '../models/access-request';
import { PagingRequest } from '../models/paging-request';

@Injectable({
  providedIn: 'root'
})
export class RequestAccessService {

  constructor(private httpClient: HttpClient) { }

  getOrganizationTypes(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations/types`);
  }

  getOrganizations(type?: string): Observable<Array<DropdownOption>> {
    if (type) {
      return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations?type=${type}`);
    } else {
      return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations`);
    }
  }

  createAccessRequest(body: AccessRequest): Observable<unknown> {
    return this.httpClient.post<unknown>(`${environment.API_HOST}/users/accessrequests`, body);
  }

  getAccessRequests(pagingRequest: PagingRequest, status: string = ''): Observable<any> {
    return this.httpClient.get(`${environment.API_HOST}/users?status=${status}&pageSize=${pagingRequest?.pageSize}&pageNumber=${pagingRequest?.pageNumber}`)
  }

  approveAccessRequest(model: ApproveRequestModel): Observable<unknown> {
    return this.httpClient.put<unknown>(`${environment.API_HOST}/users/accessrequests/approve`, model);
  }

  denyAccessRequest(model: DenyRequestModel): Observable<unknown> {
    return this.httpClient.put<unknown>(`${environment.API_HOST}/users/accessrequests/deny`, model);
  }
}
