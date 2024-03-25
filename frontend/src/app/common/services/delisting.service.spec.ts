import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { DelistingService } from './delisting.service';

describe('DelistingService', () => {
  let service: DelistingService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    service = TestBed.inject(DelistingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
