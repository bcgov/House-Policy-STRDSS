import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPlatformComponent } from './view-platform.component';

xdescribe('ViewPlatformComponent', () => {
  let component: ViewPlatformComponent;
  let fixture: ComponentFixture<ViewPlatformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewPlatformComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ViewPlatformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
