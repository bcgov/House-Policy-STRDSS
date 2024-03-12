import { TestBed } from '@angular/core/testing';

import { DelistingService } from './delisting.service';

describe('DelistingService', () => {
  let service: DelistingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DelistingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
