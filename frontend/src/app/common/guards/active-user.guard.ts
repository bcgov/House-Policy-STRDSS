
import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { map } from 'rxjs';

export const activeUserGuard: CanActivateFn = (route, state) => {

  const userDataService = inject(UserDataService)
  const router = inject(Router);

  return userDataService.getCurrentUser().pipe(
    map((user) => {
      if (user.isActive === true) {
        return true;
      }
      return router.createUrlTree(['401']);
    })
  )
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => activeUserGuard(route, state);
