import { HttpInterceptorFn } from "@angular/common/http";
import { jwtDecode } from "jwt-decode";

export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const token = localStorage.getItem('access_token');

    if (token) {
        let decoded = jwtDecode(token)
        const isExpired = decoded && decoded.exp
            ? decoded.exp < Date.now() / 1000
            : false;
        if (isExpired) {
            localStorage.removeItem('access_token');
        } else {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`,
                },
            });
        }
    }
    return next(request);
}