import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private keycloak: KeycloakService) { }

  logout(): void {
    const redirectUri = `${environment.SM_LOGOFF_URL}?returl=${encodeURIComponent(window.location.origin)}&retnow=1`;
    const logoffUri = `${this.keycloak.getKeycloakInstance().authServerUrl}/realms/${this.keycloak.getKeycloakInstance().realm
      }/protocol/openid-connect/logout?redirect_uri=${encodeURIComponent(redirectUri)}`;
    window.location.href = logoffUri;
  }
}
