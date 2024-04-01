import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlatformStaffUserScreenComponent } from './platform-staff-user-screen.component';

describe('PlatformStaffUserScreenComponent', () => {
  let component: PlatformStaffUserScreenComponent;
  let fixture: ComponentFixture<PlatformStaffUserScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlatformStaffUserScreenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PlatformStaffUserScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
