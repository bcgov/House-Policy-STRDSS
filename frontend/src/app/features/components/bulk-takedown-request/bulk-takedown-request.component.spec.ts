import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkTakedownRequestComponent } from './bulk-takedown-request.component';

xdescribe('BulkTakedownRequestComponent', () => {
  let component: BulkTakedownRequestComponent;
  let fixture: ComponentFixture<BulkTakedownRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BulkTakedownRequestComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(BulkTakedownRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
