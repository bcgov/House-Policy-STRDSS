import { TestBed } from '@angular/core/testing';

import { RequestAccessService } from './request-access.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('RequestAccessService', () => {
  let service: RequestAccessService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    service = TestBed.inject(RequestAccessService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
