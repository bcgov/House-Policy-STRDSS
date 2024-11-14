import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateLocalGovernmentInformationComponent } from './update-local-gvernment-information.component';

xdescribe('UpdateLocalGvernmentInformationComponent', () => {
  let component: UpdateLocalGovernmentInformationComponent;
  let fixture: ComponentFixture<UpdateLocalGovernmentInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateLocalGovernmentInformationComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UpdateLocalGovernmentInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
