import { TestBed } from '@angular/core/testing';

import { ListingDataService } from './listing-data.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ListingDataService', () => {
  let service: ListingDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    service = TestBed.inject(ListingDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
