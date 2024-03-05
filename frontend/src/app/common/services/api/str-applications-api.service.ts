import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PostApplication, PutApplication } from '../../models/application.model';

@Injectable({
  providedIn: 'root'
})
export class StrApplicationsApiService {
  private readonly apiUrl = `api`;

  constructor(private httpClient: HttpClient) {
  }

  getStrApplications(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}/strapplications`)
  }

  postStrApplication(application: PostApplication): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/strapplications`, application)
  }

  putStrApplication(application: PutApplication): Observable<any> {
    return this.httpClient.put(`${this.apiUrl}/strapplications`, application)
  }
}
