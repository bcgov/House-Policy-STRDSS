import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListingUploadHistoryTableComponent } from './listing-upload-history-table.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ListingUploadHistoryTableComponent', () => {
  let component: ListingUploadHistoryTableComponent;
  let fixture: ComponentFixture<ListingUploadHistoryTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ListingUploadHistoryTableComponent,
        HttpClientTestingModule,
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(ListingUploadHistoryTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
