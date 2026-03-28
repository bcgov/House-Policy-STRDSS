import { TestBed } from '@angular/core/testing';

import { UserDataService } from './user-data.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { environment } from '../../../environments/environment';
import { User } from '../models/user';
import { Router } from '@angular/router';

describe('UserDataService', () => {
  let service: UserDataService;
  let httpMock: HttpTestingController;

  const mockUser = {
    id: 1,
    userName: 'u',
    userGuid: 'g',
    identityProviderNm: 'kc',
    emailAddress: 't@test',
    firstName: 'T',
    lastName: 'U',
    fullName: 'Test User',
    displayName: 'Test',
    isActive: true,
    businessNm: '',
    accessRequestStatus: '',
    accessRequestRequired: false,
    permissions: [] as string[],
    organizationType: 'LG',
    organizationId: 1,
    termsAcceptanceDtm: '',
  } satisfies User;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [{ provide: Router, useValue: {} }],
    });
    service = TestBed.inject(UserDataService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should share one HTTP request when getCurrentUser is subscribed concurrently before cache is set', () => {
    service.getCurrentUser().subscribe((u) => expect(u.displayName).toBe('Test'));
    service.getCurrentUser().subscribe((u) => expect(u.displayName).toBe('Test'));

    const req = httpMock.expectOne(`${environment.API_HOST}/users/currentuser`);
    req.flush(mockUser);
  });

  it('invalidateCurrentUser clears cache so a new request is made', () => {
    let loads = 0;
    service.getCurrentUser().subscribe((u) => {
      loads++;
      expect(u.displayName).toBe('Test');
    });
    httpMock.expectOne(`${environment.API_HOST}/users/currentuser`).flush(mockUser);

    service.invalidateCurrentUser();

    service.getCurrentUser().subscribe((u) => {
      loads++;
      expect(u.displayName).toBe('Test');
    });
    httpMock.expectOne(`${environment.API_HOST}/users/currentuser`).flush(mockUser);

    expect(loads).toBe(2);
  });
});
