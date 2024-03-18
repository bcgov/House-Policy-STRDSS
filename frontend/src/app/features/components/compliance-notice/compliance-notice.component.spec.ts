import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplianceNoticeComponent } from './compliance-notice.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DelistingService } from '../../../common/services/delisting.service';

describe('ComplianceNoticeComponent', () => {
  let component: ComplianceNoticeComponent;
  let fixture: ComponentFixture<ComplianceNoticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplianceNoticeComponent, HttpClientTestingModule],
      providers: [DelistingService]
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
