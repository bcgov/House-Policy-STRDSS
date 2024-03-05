import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, shareReplay } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CommonCodesApiService {
  private readonly apiUrl = `api`;

  constructor(private httpClient: HttpClient) {}

  getCommonCodes(): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}/commoncodes`).pipe(shareReplay());
  }
}
