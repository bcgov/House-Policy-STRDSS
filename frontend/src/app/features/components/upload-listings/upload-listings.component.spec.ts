import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadListingsComponent } from './upload-listings.component';

xdescribe('UploadListingsComponent', () => {
  let component: UploadListingsComponent;
  let fixture: ComponentFixture<UploadListingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UploadListingsComponent]
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
