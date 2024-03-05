import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, Subject, tap } from 'rxjs';
import { AuthenticatedUser, User } from '../models/user';
import { AuthApiService } from './api/auth-api.service';
import { jwtDecode } from 'jwt-decode';
import { StoreService } from './store.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  currentUser!: AuthenticatedUser;
  userLoginSubject = new Subject<boolean>();

  constructor(
    private router: Router,
    private authApiService: AuthApiService,
    private storeService: StoreService) { }

  canActivate(_next: ActivatedRouteSnapshot, _state: RouterStateSnapshot): boolean | UrlTree {
    let token = localStorage.getItem('access_token');
    let userdata = localStorage.getItem('user_data');

    if (userdata) {
      this.currentUser = JSON.parse(userdata);
    }

    if (token) {
      let decoded = jwtDecode(token);
      const isExpired = decoded && decoded.exp
        ? decoded.exp < Date.now() / 1000
        : false;
      if (isExpired) {
        localStorage.removeItem('access_token');
        localStorage.removeItem('user_data');
      } else {
        this.userLoginSubject.next(true);
        return true;
      }
    }

    this.userLoginSubject.next(false);
    return this.router.createUrlTree(['login']);
  }

  signIn(username: string, password: string): Observable<any> {
    return this.authApiService.login(username, password).pipe(tap({
      next: (value) => {
        localStorage.setItem('access_token', value.token);
        localStorage.setItem('user_data', JSON.stringify(value));
        this.storeService.currentUser = value;
        this.userLoginSubject.next(true);
      }
    }));
  }

  signUp(user: User): Observable<any> {
    return this.authApiService.signup(user);
  }

  signOut(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('user_data');
    this.storeService.currentUser = null;
    this.userLoginSubject.next(false);
  }
}

export const AuthGuard: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree => {
  return inject(AuthenticationService).canActivate(next, state);
}