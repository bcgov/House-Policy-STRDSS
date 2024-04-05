import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { approvedUserGuard } from './approved-user.guard';

describe('approvedUserGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => approvedUserGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
