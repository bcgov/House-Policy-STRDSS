import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CeuStaffUserScreenComponent } from './ceu-staff-user-screen.component';

describe('CeuStaffUserScreenComponent', () => {
  let component: CeuStaffUserScreenComponent;
  let fixture: ComponentFixture<CeuStaffUserScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CeuStaffUserScreenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CeuStaffUserScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
