import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CeuAdminUserScreenComponent } from './ceu-admin-user-screen.component';

describe('CeuAdminUserScreenComponent', () => {
  let component: CeuAdminUserScreenComponent;
  let fixture: ComponentFixture<CeuAdminUserScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CeuAdminUserScreenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CeuAdminUserScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
