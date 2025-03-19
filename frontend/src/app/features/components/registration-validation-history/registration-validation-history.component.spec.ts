import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationValidationHistoryComponent } from './registration-validation-history.component';

describe('RegistrationValidationHistoryComponent', () => {
  let component: RegistrationValidationHistoryComponent;
  let fixture: ComponentFixture<RegistrationValidationHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistrationValidationHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegistrationValidationHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
