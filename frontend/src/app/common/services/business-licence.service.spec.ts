import { TestBed } from '@angular/core/testing';

import { BusinessLicenceService } from './business-licence.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

xdescribe('BusinessLicenceService', () => {
  let service: BusinessLicenceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(BusinessLicenceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
