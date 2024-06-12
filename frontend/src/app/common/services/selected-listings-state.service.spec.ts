import { TestBed } from '@angular/core/testing';

import { SelectedListingsStateService } from './selected-listings-state.service';

describe('SelectedListingsStateService', () => {
  let service: SelectedListingsStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SelectedListingsStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
