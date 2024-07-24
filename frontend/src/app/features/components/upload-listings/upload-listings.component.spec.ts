import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadListingsComponent } from './upload-listings.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MessageService } from 'primeng/api';

describe('UploadListingsComponent', () => {
  let component: UploadListingsComponent;
  let fixture: ComponentFixture<UploadListingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        UploadListingsComponent,
        HttpClientTestingModule,
      ],
      providers: [MessageService],
    })
      .compileComponents();

    fixture = TestBed.createComponent(UploadListingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
