import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewPlatformComponent } from './add-new-platform.component';

xdescribe('AddNewPlatformComponent', () => {
  let component: AddNewPlatformComponent;
  let fixture: ComponentFixture<AddNewPlatformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNewPlatformComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AddNewPlatformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
