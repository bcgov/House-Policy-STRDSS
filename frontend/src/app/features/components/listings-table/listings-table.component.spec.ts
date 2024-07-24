import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListingsTableComponent } from './listings-table.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('ListingsTableComponent', () => {
  let component: ListingsTableComponent;
  let fixture: ComponentFixture<ListingsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ListingsTableComponent,
        HttpClientTestingModule,
        NoopAnimationsModule,
      ], providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of(),
          }
        }
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(ListingsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
