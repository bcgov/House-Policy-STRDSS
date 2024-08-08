import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})

// before KC Quarkus update

export class AuthService {

  constructor(private keycloak: KeycloakService) { }

  logout(): void {
    const redirectUri = `${environment.SM_LOGOFF_URL}?returl=${encodeURIComponent(window.location.origin)}&retnow=1`;
    const logoffUri = `${this.keycloak.getKeycloakInstance().authServerUrl}/realms/${this.keycloak.getKeycloakInstance().realm
      }/protocol/openid-connect/logout?redirect_uri=${encodeURIComponent(redirectUri)}`;
    window.location.href = logoffUri;
  }
}

// after KC Quarkus update

// export class AuthService {
//   constructor(private keycloak: KeycloakService) {}

//   async logout(): Promise<void> {
//       try {
//           const idToken = await this.keycloak.getToken();
//           const redirectUri = `${environment.SM_LOGOFF_URL}?returl=${encodeURIComponent(
//               window.location.origin,
//           )}&retnow=1`;
//           const logoffUri = `${this.keycloak.getKeycloakInstance().authServerUrl}/realms/${
//               this.keycloak.getKeycloakInstance().realm
//           }/protocol/openid-connect/logout?post_logout_redirect_uri=${encodeURIComponent(
//               redirectUri,
//           )}&id_token_hint=${idToken}`;
//           window.location.href = logoffUri;
//       } catch (error) {
//           console.error('Failed to get ID token', error);
//       }
//   }
// }

