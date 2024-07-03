import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkTakedownRequestComponent } from './bulk-takedown-request.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('BulkTakedownRequestComponent', () => {
  let component: BulkTakedownRequestComponent;
  let fixture: ComponentFixture<BulkTakedownRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BulkTakedownRequestComponent,
        HttpClientTestingModule,
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

    fixture = TestBed.createComponent(BulkTakedownRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
