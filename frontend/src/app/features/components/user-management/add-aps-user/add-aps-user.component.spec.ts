import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddApsUserComponent } from './add-aps-user.component';

xdescribe('AddApsUserComponent', () => {
  let component: AddApsUserComponent;
  let fixture: ComponentFixture<AddApsUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddApsUserComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AddApsUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
