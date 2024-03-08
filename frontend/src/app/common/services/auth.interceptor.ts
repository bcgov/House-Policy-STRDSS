import { HttpEvent, HttpHandlerFn, HttpHeaders, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { KeycloakService } from "keycloak-angular";
import { Observable, mergeMap } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    const keykloakService = inject(KeycloakService);

    return keykloakService.addTokenToHeader(req.headers).pipe(
        mergeMap((headers: HttpHeaders) => {
            return next(req.clone({ headers: headers }));
        }));


}