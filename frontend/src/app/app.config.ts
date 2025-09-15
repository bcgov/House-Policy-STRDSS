import { APP_INITIALIZER, ApplicationConfig, CSP_NONCE, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { KeycloakAngularModule, KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './common/services/auth.interceptor';
import { MessageService } from 'primeng/api';
import { errorInterceptor } from './common/consts/error-interceptor.const';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';

export const appConfig: ApplicationConfig = {
    providers: [
        provideAnimations(),
        provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
        provideRouter(routes),
        // Provide CSP_NONCE first so it can be injected by other providers
        {
            provide: CSP_NONCE,
            useFactory: () => document.querySelector('meta[name="csp-nonce"]')?.getAttribute('content'),
        },
        // Use standard PrimeNG provider
        providePrimeNG({
            theme: {
                preset: Aura,
                options: {
                    prefix: 'p',
                    darkModeSelector: false,
                    cssLayer: false
                }
            },
            csp: {
                nonce: '...'
            }
        }),
        // Use modern provider pattern for Keycloak
        importProvidersFrom(KeycloakAngularModule),
        {
            provide: APP_INITIALIZER,
            useFactory: initializeKeycloak,
            multi: true,
            deps: [KeycloakService],
        },
        MessageService,
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
                silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
            },
        }).then(() => {
            setupTokenRefresh(keycloak);
        });
}

function setupTokenRefresh(keycloak: KeycloakService) {
    const refreshInterval = 60 * 1000; // Check every 60 seconds

    setInterval(async () => {
        if (keycloak.isLoggedIn()) {
            try {
                const refreshed = await keycloak.updateToken(30); // Refresh if token will expire in 30 seconds
                if (refreshed) {
                    console.log('Token refreshed successfully');
                }
            } catch (err) {
                console.error('Failed to refresh token', err);
                keycloak.logout();
            }
        }
    }, refreshInterval);
}
