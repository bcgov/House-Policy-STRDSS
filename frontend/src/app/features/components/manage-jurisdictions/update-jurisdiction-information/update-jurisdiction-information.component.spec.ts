import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateJurisdictionInformationComponent } from './update-jurisdiction-information.component';

xdescribe('UpdateJurisdictionInformationComponent', () => {
  let component: UpdateJurisdictionInformationComponent;
  let fixture: ComponentFixture<UpdateJurisdictionInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateJurisdictionInformationComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UpdateJurisdictionInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
