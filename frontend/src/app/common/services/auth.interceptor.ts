import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpHeaders, HttpInterceptorFn, HttpRequest, HttpResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { KeycloakService } from "keycloak-angular";
import { EMPTY, Observable, catchError, mergeMap, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    const keykloakService = inject(KeycloakService);
    const router = inject(Router);

    return keykloakService.addTokenToHeader(req.headers).pipe(
        mergeMap((headers: HttpHeaders) => {
            return next(req.clone({ headers: headers })).pipe(
                catchError((error: HttpErrorResponse) => {
                    if (error.status === 401) {
                        if (error.error.title === 'Unauthorized') {
                            router.navigateByUrl('/401');
                        }

                        return EMPTY;
                    } else {
                        return throwError(() => error);
                    }
                }),
            )
        })
    );
}
