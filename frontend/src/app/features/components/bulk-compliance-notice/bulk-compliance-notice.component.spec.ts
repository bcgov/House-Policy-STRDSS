import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkComplianceNoticeComponent } from './bulk-compliance-notice.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('BulkComplianceNoticeComponent', () => {
  let component: BulkComplianceNoticeComponent;
  let fixture: ComponentFixture<BulkComplianceNoticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BulkComplianceNoticeComponent,
        HttpClientTestingModule,
        NoopAnimationsModule,
      ], providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of(),
          }
        }
      ],
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
