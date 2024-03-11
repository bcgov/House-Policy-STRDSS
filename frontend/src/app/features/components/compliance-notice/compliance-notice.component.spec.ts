import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplianceNoticeComponent } from './compliance-notice.component';

describe('ComplianceNoticeComponent', () => {
  let component: ComplianceNoticeComponent;
  let fixture: ComponentFixture<ComplianceNoticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplianceNoticeComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ComplianceNoticeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
