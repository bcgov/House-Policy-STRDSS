import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LgStaffUserScreenComponent } from './lg-staff-user-screen.component';

describe('LgStaffUserScreenComponent', () => {
  let component: LgStaffUserScreenComponent;
  let fixture: ComponentFixture<LgStaffUserScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LgStaffUserScreenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LgStaffUserScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
