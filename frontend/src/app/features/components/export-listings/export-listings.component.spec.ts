import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportListingsComponent } from './export-listings.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ExportListingsComponent', () => {
  let component: ExportListingsComponent;
  let fixture: ComponentFixture<ExportListingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ExportListingsComponent,
        HttpClientTestingModule,
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(ExportListingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
