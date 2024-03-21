import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccessRequestComponent } from './access-request.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('AccessRequestComponent', () => {
  let component: AccessRequestComponent;
  let fixture: ComponentFixture<AccessRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessRequestComponent, HttpClientTestingModule]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AccessRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
