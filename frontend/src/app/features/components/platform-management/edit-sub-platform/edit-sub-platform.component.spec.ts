import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditSubPlatformComponent } from './edit-sub-platform.component';

xdescribe('EditSubPlatformComponent', () => {
  let component: EditSubPlatformComponent;
  let fixture: ComponentFixture<EditSubPlatformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditSubPlatformComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(EditSubPlatformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
