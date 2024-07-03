import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserManagementComponent } from './user-management.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('UserListComponent', () => {
  let component: UserManagementComponent;
  let fixture: ComponentFixture<UserManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        UserManagementComponent,
        HttpClientTestingModule,
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
