import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendComplianceOrderComponent } from './send-compliance-order.component';

xdescribe('SendComplianceOrderComponent', () => {
  let component: SendComplianceOrderComponent;
  let fixture: ComponentFixture<SendComplianceOrderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SendComplianceOrderComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(SendComplianceOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
