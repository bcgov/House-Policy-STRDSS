import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AccessRequest, ApproveRequestModel, DenyRequestModel } from '../models/access-request';
import { PagingRequest } from '../models/paging-request';

@Injectable({
  providedIn: 'root'
})
export class RequestAccessService {

  constructor(private httpClient: HttpClient) { }

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
