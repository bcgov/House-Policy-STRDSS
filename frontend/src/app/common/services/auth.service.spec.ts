import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { KeycloakService } from 'keycloak-angular';
import { UserDataService } from './user-data.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: KeycloakService,
          useValue: {

          }
        },
        {
          provide: UserDataService,
          useValue: {
            invalidateCurrentUser: jasmine.createSpy('invalidateCurrentUser'),
          },
        },
      ],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
