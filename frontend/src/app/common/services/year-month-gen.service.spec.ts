import { TestBed } from '@angular/core/testing';

import { YearMonthGenService } from './year-month-gen.service';

xdescribe('YearMonthGenService', () => {
  let service: YearMonthGenService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(YearMonthGenService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
