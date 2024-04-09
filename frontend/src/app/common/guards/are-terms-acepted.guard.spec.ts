import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { areTermsAceptedGuard } from './are-terms-acepted.guard';

describe('areTermsAceptedGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => areTermsAceptedGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
