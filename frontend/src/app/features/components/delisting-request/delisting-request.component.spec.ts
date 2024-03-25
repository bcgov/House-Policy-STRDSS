import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { DelistingRequestComponent } from './delisting-request.component';
import { DelistingService } from '../../../common/services/delisting.service';

describe('DelistingRequestComponent', () => {
  let component: DelistingRequestComponent;
  let fixture: ComponentFixture<DelistingRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DelistingRequestComponent, HttpClientTestingModule],
      providers: [DelistingService]
    })
      .compileComponents();

    fixture = TestBed.createComponent(DelistingRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
