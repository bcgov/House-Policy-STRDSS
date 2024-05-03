import { APP_INITIALIZER, ApplicationConfig, CSP_NONCE } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './common/services/auth.interceptor';
import { MessageService } from 'primeng/api';
import { errorInterceptor } from './common/consts/error-interceptor.const';
export const appConfig: ApplicationConfig = {
    providers: [
        provideAnimations(),
        provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
        provideRouter(routes),
        KeycloakService,
        {
            provide: APP_INITIALIZER,
            useFactory: initializeKeycloak,
            multi: true,
            deps: [KeycloakService],
        },
        MessageService,
        {
            provide: CSP_NONCE,
            useValue: document.querySelector('meta[name="csp-nonce"]')?.getAttribute('content'),
        },
    ],
}

function initializeKeycloak(keycloak: KeycloakService) {
    return () =>
        keycloak.init({
            config: {
                url: environment.SSO_HOST,
                realm: environment.SSO_REALM,
                clientId: environment.SSO_CLIENT,
            },
            initOptions: {
                onLoad: 'login-required',
                pkceMethod: 'S256',
            },
        });
}
