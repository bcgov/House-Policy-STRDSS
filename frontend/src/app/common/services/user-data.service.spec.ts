import { TestBed } from '@angular/core/testing';

import { UserDataService } from './user-data.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

xdescribe('UserDataService', () => {
  let service: UserDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    service = TestBed.inject(UserDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
