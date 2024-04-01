import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption } from '../models/dropdown-option';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {

  constructor(private httpClient: HttpClient) { }

  getCurrentUser(): Observable<unknown> {
    return this.httpClient.get<unknown>(`${environment.API_HOST}/users/currentuser`);
  }

  getUsers(status: string, search: string, organizationId: number | null, pageSize: number, pageNumber: number, orderBy: string, direction: 'asc' | 'desc'): Observable<any> {
    return this.httpClient.get(
      `${environment.API_HOST}/users?status=${status ?? ''}&search=${search ?? ''}&organizationId=${organizationId ?? ''}&pageSize=${pageSize ?? ''}&pageNumber=${pageNumber ?? ''}&orderBy=${orderBy ?? ''}&direction=${direction ?? ''}`
    );
  }

  updateIsEnabled(userIdentityId: number, isEnabled: boolean, updDtm: string): Observable<void> {
    return this.httpClient.put<void>(`${environment.API_HOST}/users/updateisenabled`, { userIdentityId, isEnabled, updDtm });
  }

  getStatuses(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/users/accessrequeststatuses`);
  }
}
