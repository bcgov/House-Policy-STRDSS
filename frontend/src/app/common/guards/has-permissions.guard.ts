import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { Observable, map } from 'rxjs';

export const hasPermissionsGuard: CanActivateFn = (route, _state): Observable<boolean | UrlTree> | boolean | UrlTree => {
  const userDataService = inject(UserDataService)
  const router = inject(Router);

  return userDataService.getCurrentUser().pipe(
    map((user) => {

      let hasPerm;
      route.data['permissions'].forEach((requiredPerm: string) => {
        hasPerm = user.permissions.includes(requiredPerm);
      });
      if (hasPerm) {
        return true;
      } else {
        return router.createUrlTree(['401']);
      }
    })
  );
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => hasPermissionsGuard(route, state);
