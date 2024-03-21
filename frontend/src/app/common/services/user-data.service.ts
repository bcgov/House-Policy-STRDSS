import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {

  constructor(private httpClient: HttpClient) { }

  getCurrentUser(): Observable<unknown> {
    return this.httpClient.get<unknown>(`${environment.API_HOST}/users/currentuser`)
  }
}
