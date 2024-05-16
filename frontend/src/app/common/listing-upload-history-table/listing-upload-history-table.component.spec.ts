import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListingUploadHistoryTableComponent } from './listing-upload-history-table.component';

xdescribe('ListingUploadHistoryTableComponent', () => {
  let component: ListingUploadHistoryTableComponent;
  let fixture: ComponentFixture<ListingUploadHistoryTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListingUploadHistoryTableComponent]
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
