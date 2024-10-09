import { Injectable } from '@angular/core';
import { DropdownOption } from '../models/dropdown-option';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Platform } from '../models/platform';
import { PagingResponse } from '../models/paging-response';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {

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

  getPlatforms(pageSize = 1000, pageNumber = 1, orderBy?: string, direction?: 'asc'): Observable<PagingResponse<Platform>> {

    let url = `${environment.API_HOST}/organizations/platforms?pageSize=${pageSize}&pageNumber=${pageNumber}`;

    if (orderBy) {
      url += `&orderBy=${orderBy}&direction=${direction || 'asc'}`;
    }

    return this.httpClient.get<PagingResponse<Platform>>(url);
  }

  getPlatform(id: number): Observable<Platform> {
    return this.httpClient.get<Platform>(`${environment.API_HOST}/organizations/platform/${id}`);
  }

  addPlatform(platform: Platform): Observable<Platform> {
    return this.httpClient.post<Platform>(`${environment.API_HOST}/organizations/platforms`, platform);
  }

  addSubPlatform(platform: Platform, parentId: number): Observable<Platform> {
    return this.httpClient.post<Platform>(`${environment.API_HOST}/organizations/platform/${parentId}`, platform);
  }

  editPlatform(platform: Platform): Observable<Platform> {
    return this.httpClient.put<Platform>(`${environment.API_HOST}/organizations/platform`, platform);
  }
}
