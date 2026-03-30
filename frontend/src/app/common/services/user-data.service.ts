import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, finalize, of, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption } from '../models/dropdown-option';
import { User, UserDetails } from '../models/user';
import { Router } from '@angular/router';
import { ApsUser } from '../models/aps-user';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {
  currentUser?: User;
  private currentUserRequest$?: Observable<User>;

  constructor(private httpClient: HttpClient, private router: Router) { }

  getCurrentUser(): Observable<User> {
    if (this.currentUser) {
      return of(this.currentUser);
    }
    if (!this.currentUserRequest$) {
      this.currentUserRequest$ = this.httpClient
        .get<User>(`${environment.API_HOST}/users/currentuser`)
        .pipe(
          tap((user) => {
            this.currentUser = user;
          }),
          shareReplay({ bufferSize: 1, refCount: true }),
          finalize(() => {
            this.currentUserRequest$ = undefined;
          })
        );
    }
    return this.currentUserRequest$;
  }

  /** Clears cached profile so the next getCurrentUser() hits the API. Call on logout and when server-side user state may have changed without a full reload. */
  invalidateCurrentUser(): void {
    this.currentUser = undefined;
    this.currentUserRequest$ = undefined;
  }

  getUsers(status: string, search: string, organizationId: number | null, pageSize: number, pageNumber: number, orderBy: string, direction: 'asc' | 'desc'): Observable<any> {
    return this.httpClient.get(
      `${environment.API_HOST}/users?status=${status ?? ''}&search=${search ?? ''}&organizationId=${organizationId ?? ''}&pageSize=${pageSize ?? ''}&pageNumber=${pageNumber ?? ''}&orderBy=${orderBy ?? ''}&direction=${direction ?? ''}`
    );
  }

  getUserById(userId: number): Observable<UserDetails> {
    return this.httpClient.get<UserDetails>(`${environment.API_HOST}/users/${userId}`);
  }

  updateUser(data: {
    userIdentityId: number,
    representedByOrganizationId: string,
    roleCds: Array<string>,
    isEnabled: boolean,
    updDtm: string
  }): Observable<any> {
    return this.httpClient.put<UserDetails>(`${environment.API_HOST}/users/${data.userIdentityId}`, data)
  }

  createApsUser(user: ApsUser): Observable<any> {
    return this.httpClient.post(`${environment.API_HOST}/users/aps`, user)
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
