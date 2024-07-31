import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadBusinessLicenseComponent } from './upload-business-license.component';
import { BusinessLicenceService } from '../../../common/services/business-licence.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('UploadBusinessLicenseComponent', () => {
  let component: UploadBusinessLicenseComponent;
  let fixture: ComponentFixture<UploadBusinessLicenseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        UploadBusinessLicenseComponent,
        HttpClientTestingModule,
      ],
      providers: [BusinessLicenceService]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UploadBusinessLicenseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
