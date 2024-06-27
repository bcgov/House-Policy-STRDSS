import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserPermission, UserRole } from '../models/user-role';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class RolesService {

  constructor(private httpClient: HttpClient) { }

  getRoles(): Observable<Array<UserRole>> {
    return this.httpClient.get<Array<UserRole>>(`${environment.API_HOST}/roles`)
  }

  getRole(id: string): Observable<UserRole> {
    return this.httpClient.get<UserRole>(`${environment.API_HOST}/roles/${id}`)
  }

  updateRole(role: UserRole): Observable<UserRole> {
    return this.httpClient.put<UserRole>(`${environment.API_HOST}/roles/${role.userRoleCd}`, {
      ...role, permissions: role.permissions.map((perm) => {
        return perm.userPrivilegeCd
      })
    })
  }

  createRole(role: UserRole): Observable<any> {
    return this.httpClient.post<any>(`${environment.API_HOST}/roles`, {
      ...role, permissions: role.permissions.map((perm) => {
        return perm.userPrivilegeCd
      })
    })
  }

  deleteRole(id: string): Observable<UserRole> {
    return this.httpClient.delete<UserRole>(`${environment.API_HOST}/roles/${id}`)
  }

  getPermissions(): Observable<Array<UserPermission>> {
    return this.httpClient.get<Array<UserPermission>>(`${environment.API_HOST}/roles/permissions`)
  }
}
