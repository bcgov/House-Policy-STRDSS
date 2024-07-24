import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesListComponent } from './roles-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('RolesListComponent', () => {
  let component: RolesListComponent;
  let fixture: ComponentFixture<RolesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RolesListComponent,
        HttpClientTestingModule,
      ], providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              params: of({ id: 123 })
            }
          }
        }
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(RolesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
