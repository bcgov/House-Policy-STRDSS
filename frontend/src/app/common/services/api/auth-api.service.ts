import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticatedUser, User } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  private readonly apiUrl = `api`;

  constructor(private httpClient: HttpClient) {
  }

  login(username: string, password: string): Observable<AuthenticatedUser> {
    return this.httpClient.post<AuthenticatedUser>(`${this.apiUrl}/systemusers/login`, { username, password });
  }

  signup(user: User): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/systemusers`, user)
  }
}
