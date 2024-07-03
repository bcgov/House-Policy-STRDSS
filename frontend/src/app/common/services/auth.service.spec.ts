import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { KeycloakService } from 'keycloak-angular';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: KeycloakService,
          useValue: {

          }
        }
      ],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
