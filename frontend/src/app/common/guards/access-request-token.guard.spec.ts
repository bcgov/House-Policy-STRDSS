import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { accessRequestTokenGuard } from './access-request-token.guard';

describe('accessRequestTokenGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => accessRequestTokenGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
