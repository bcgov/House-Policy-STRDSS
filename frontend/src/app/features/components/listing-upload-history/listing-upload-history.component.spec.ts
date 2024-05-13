import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListingUploadHistoryComponent } from './listing-upload-history.component';

describe('ListingUploadHistoryComponent', () => {
  let component: ListingUploadHistoryComponent;
  let fixture: ComponentFixture<ListingUploadHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListingUploadHistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ListingUploadHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
