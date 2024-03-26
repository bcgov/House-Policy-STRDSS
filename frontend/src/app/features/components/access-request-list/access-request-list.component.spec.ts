import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccessRequestListComponent } from './access-request-list.component';

xdescribe('AccessRequestListComponent', () => {
  let component: AccessRequestListComponent;
  let fixture: ComponentFixture<AccessRequestListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessRequestListComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AccessRequestListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
