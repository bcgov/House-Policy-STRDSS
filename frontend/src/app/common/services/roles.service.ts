import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserPermission, UserRole } from '../models/user-role';

@Injectable({
  providedIn: 'root'
})
export class RolesService {

  constructor(private httpClient: HttpClient) { }

  getRoles(): Observable<Array<UserRole>> {
    return this.httpClient.get<Array<UserRole>>(`${environment.API_HOST}/roles`)
  }

  getRolesById(id: any): Observable<UserRole> {
    return this.httpClient.get<UserRole>(`${environment.API_HOST}/roles/${id}`)
  }

  getPermissions(): Observable<Array<UserPermission>> {
    return this.httpClient.get<Array<UserPermission>>(`${environment.API_HOST}/roles/permissions`)
  }
}
