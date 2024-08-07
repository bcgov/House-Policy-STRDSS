import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { EMPTY, catchError, throwError } from 'rxjs';
import { ErrorHandlingService } from '../services/error-handling.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const errorService = inject(ErrorHandlingService);

    return next(req).pipe(
        catchError((err: any) => {
            if (err instanceof HttpErrorResponse) {

                switch (err.status) {
                    case 422:
                        errorService.showErrorFromBackend(err.error as any);
                        return EMPTY;
                    case 500:
                        errorService.showErrorFromBackend(err as any);
                        return throwError(() => err);

                    default:
                        break;
                }

            } else {
                console.error('An error occurred:', err);
            }

            return throwError(() => err);
        })
    );;
};