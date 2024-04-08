import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption } from '../models/dropdown-option';
import { User } from '../models/user';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {
  currentUser!: User;

  constructor(private httpClient: HttpClient, private router: Router) { }

  getCurrentUser(): Observable<User> {
    return this.currentUser
      ? of(this.currentUser)
      : this.httpClient.get<User>(`${environment.API_HOST}/users/currentuser`).pipe(
        tap(user => {
          this.currentUser = user;
        })
      );
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

  acceptTermsAndConditions(): Observable<unknown> {
    return this.httpClient.put<void>(`${environment.API_HOST}/users/accepttermsconditions`, {});
  }
}
