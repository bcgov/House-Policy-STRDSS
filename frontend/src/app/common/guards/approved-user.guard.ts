import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { Observable, map } from 'rxjs';

export const approvedUserGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> | boolean | UrlTree => {
  const userDataService = inject(UserDataService)
  const router = inject(Router);

  return userDataService.getCurrentUser().pipe(
    map((user) => {
      if (user.accessRequestStatus === 'Approved') {
        return true;
      }
      return router.createUrlTree(['401']);
    })
  )

};
export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => approvedUserGuard(route, state);
