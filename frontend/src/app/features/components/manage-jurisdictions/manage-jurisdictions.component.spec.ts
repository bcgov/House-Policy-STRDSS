import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageJurisdictionsComponent } from './manage-jurisdictions.component';

xdescribe('ManageJurisdictionsComponent', () => {
  let component: ManageJurisdictionsComponent;
  let fixture: ComponentFixture<ManageJurisdictionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageJurisdictionsComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ManageJurisdictionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
