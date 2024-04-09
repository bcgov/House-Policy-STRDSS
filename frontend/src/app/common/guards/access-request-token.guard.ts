import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { map } from 'rxjs';

export const accessRequestTokenGuard: CanActivateFn = (route, _state) => {
  const userDataService = inject(UserDataService)
  const router = inject(Router);

  return userDataService.getCurrentUser().pipe(
    map((user) => {
      if (user.accessRequestStatus === "Requested" || user.accessRequestStatus === "Approved") {
        return router.createUrlTree(['401']);
      }

      if (route.routeConfig?.path) {
        if (route.routeConfig?.path === 'access-request') {
          if (route.queryParams?.['token']) {
            return true;
          }
        }
      }
      return router.createUrlTree(['401']);
    }),
  );
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => accessRequestTokenGuard(route, state);
