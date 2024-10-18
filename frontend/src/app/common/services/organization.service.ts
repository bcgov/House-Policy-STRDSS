import { Injectable } from '@angular/core';
import { DropdownOption } from '../models/dropdown-option';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Platform, SubPlatform, SubPlatformCreate, UpdatePlatform, UpdateSubPlatform } from '../models/platform';
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

  getPlatform(id: number): Observable<Platform | SubPlatform> {
    return this.httpClient.get<Platform | SubPlatform>(`${environment.API_HOST}/organizations/platforms/${id}`);
  }

  addPlatform(platform: Platform): Observable<any> {
    return this.httpClient.post<any>(`${environment.API_HOST}/organizations/platforms`, platform);
  }

  addSubPlatform(platform: SubPlatformCreate): Observable<any> {
    return this.httpClient.post<any>(`${environment.API_HOST}/organizations/platforms/subsidiaries`, platform);
  }

  editPlatform(id: number, platform: UpdatePlatform): Observable<any> {
    return this.httpClient.put<any>(`${environment.API_HOST}/organizations/platforms/${id}`, platform);
  }

  editSubPlatform(id: number, platform: UpdateSubPlatform): Observable<any> {
    return this.httpClient.put<any>(`${environment.API_HOST}/organizations/platforms/subsidiaries/${id}`, platform);
  }

  getPlatformTypes(): Observable<Array<DropdownOption>> {
    return this.httpClient.get<Array<DropdownOption>>(`${environment.API_HOST}/organizations/platformTypeDropdown`);
  }
}
