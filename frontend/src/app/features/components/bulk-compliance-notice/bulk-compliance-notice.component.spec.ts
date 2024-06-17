import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkComplianceNoticeComponent } from './bulk-compliance-notice.component';

xdescribe('BulkComplianceNoticeComponent', () => {
  let component: BulkComplianceNoticeComponent;
  let fixture: ComponentFixture<BulkComplianceNoticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BulkComplianceNoticeComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(BulkComplianceNoticeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
