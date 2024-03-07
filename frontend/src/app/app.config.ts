import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { provideAnimations } from '@angular/platform-browser/animations'
export const appConfig: ApplicationConfig = {
    providers: [
        provideAnimations(),
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
