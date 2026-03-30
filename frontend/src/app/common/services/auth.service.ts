import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../../environments/environment';
import { UserDataService } from './user-data.service';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

  constructor(
    private keycloak: KeycloakService,
    private userDataService: UserDataService,
  ) { }

  logout(): void {
    this.userDataService.invalidateCurrentUser();
    const redirectUri = `${environment.SM_LOGOFF_URL}?returl=${encodeURIComponent(window.location.origin)}&retnow=1`;
    const logoffUri = `${this.keycloak.getKeycloakInstance().authServerUrl}/realms/${this.keycloak.getKeycloakInstance().realm
      }/protocol/openid-connect/logout?redirect_uri=${encodeURIComponent(redirectUri)}`;
    window.location.href = logoffUri;
  }
}
