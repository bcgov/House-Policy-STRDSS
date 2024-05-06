import { TestBed } from '@angular/core/testing';

import { TopMenuService } from './top-menu.service';

xdescribe('TopMenuService', () => {
  let service: TopMenuService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TopMenuService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
