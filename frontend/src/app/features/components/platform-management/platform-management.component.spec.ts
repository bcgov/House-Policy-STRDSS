import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlatformManagementComponent } from './platform-management.component';

xdescribe('PlatformManagementComponent', () => {
  let component: PlatformManagementComponent;
  let fixture: ComponentFixture<PlatformManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlatformManagementComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(PlatformManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
