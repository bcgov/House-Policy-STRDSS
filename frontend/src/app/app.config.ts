import { APP_INITIALIZER, ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { provideAnimations } from '@angular/platform-browser/animations'
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './common/services/auth.interceptor';
export const appConfig: ApplicationConfig = {
    providers: [
        provideAnimations(),
        provideHttpClient(
            withInterceptors([authInterceptor])
        ),
        provideRouter(routes),
        KeycloakService,
        {
            provide: APP_INITIALIZER,
            useFactory: initializeKeycloak,
            multi: true,
            deps: [KeycloakService],
        },
    ],
};

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
                // silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
            },
        });
}
