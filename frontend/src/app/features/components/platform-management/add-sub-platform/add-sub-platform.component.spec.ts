import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSubPlatformComponent } from './add-sub-platform.component';

xdescribe('AddSubPlatformComponent', () => {
  let component: AddSubPlatformComponent;
  let fixture: ComponentFixture<AddSubPlatformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddSubPlatformComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AddSubPlatformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
