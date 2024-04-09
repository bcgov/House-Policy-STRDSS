import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { map } from 'rxjs';

export const areTermsAceptedGuard: CanActivateFn = (_route, _state) => {
  const userDataService = inject(UserDataService);
  const router = inject(Router);

  return userDataService.getCurrentUser().pipe(
    map((user) => {
      if (user.termsAcceptanceDtm) {
        return true;
      }
      return router.createUrlTree(['terms-and-conditions']);
    })
  );
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => areTermsAceptedGuard(route, state);
