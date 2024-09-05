import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AggregatedListingsTableComponent } from './aggregated-listings-table.component';

xdescribe('AggregatedListingsTableComponent', () => {
  let component: AggregatedListingsTableComponent;
  let fixture: ComponentFixture<AggregatedListingsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AggregatedListingsTableComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AggregatedListingsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
