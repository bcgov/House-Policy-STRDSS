import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateLocalGvernmentInformationComponent } from './update-local-gvernment-information.component';

xdescribe('UpdateLocalGvernmentInformationComponent', () => {
  let component: UpdateLocalGvernmentInformationComponent;
  let fixture: ComponentFixture<UpdateLocalGvernmentInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateLocalGvernmentInformationComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UpdateLocalGvernmentInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
