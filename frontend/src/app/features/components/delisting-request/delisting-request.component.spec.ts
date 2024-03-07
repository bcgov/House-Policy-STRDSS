import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DelistingRequestComponent } from './delisting-request.component';

describe('DelistingRequestComponent', () => {
  let component: DelistingRequestComponent;
  let fixture: ComponentFixture<DelistingRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DelistingRequestComponent]
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
