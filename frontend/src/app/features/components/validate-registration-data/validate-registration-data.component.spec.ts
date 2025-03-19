import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidateRegistrationDataComponent } from './validate-registration-data.component';

describe('ValidateRegistrationDataComponent', () => {
  let component: ValidateRegistrationDataComponent;
  let fixture: ComponentFixture<ValidateRegistrationDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ValidateRegistrationDataComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ValidateRegistrationDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
