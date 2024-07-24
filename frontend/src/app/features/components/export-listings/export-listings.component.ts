import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagesModule } from 'primeng/messages';
import { ListingDataService } from '../../../common/services/listing-data.service';
import { ExportJurisdiction } from '../../../common/models/export-listing';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-export-listings',
  standalone: true,
  imports: [
    RadioButtonModule,
    ButtonModule,
    CommonModule,
    FormsModule,
    MessagesModule,
  ],
  templateUrl: './export-listings.component.html',
  styleUrl: './export-listings.component.scss'
})
export class ExportListingsComponent implements OnInit {
  dateLastUpdated = '';
  jurisdictions = new Array<ExportJurisdiction>();
  selectedJurisdiction!: any;

  constructor(
    private cd: ChangeDetectorRef,
    private errorService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
    private listingService: ListingDataService,
  ) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.listingService.getJurisdictions().subscribe({
      next: (jurisdictions) => {
        console.log(jurisdictions);

        if (jurisdictions) {
          this.jurisdictions = jurisdictions;
          this.dateLastUpdated = this.jurisdictions[0].updDtm;
        }
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  onDownload(): void {
    this.loaderService.loadingStart();
    this.listingService.downloadListings(this.selectedJurisdiction).subscribe({
      next: (content) => {
        const jurisdiction = this.jurisdictions.filter(x => x.rentalListingExtractId === this.selectedJurisdiction)[0];
        const date = new Date(jurisdiction.updDtm);

        var blob = new Blob([content]);
        var url = window.URL.createObjectURL(blob);

        const element = document.createElement('a');
        element.setAttribute('href', url);
        element.setAttribute('download', `STRlisting_${jurisdiction.rentalListingExtractNm}_${formatDate(date, 'yyyyMMdd', 'en-US')}.zip`);

        element.click();
        element.remove();
        window.URL.revokeObjectURL(url);

        this.errorService.showSuccess('Your Listing Data was Successfully Downloaded.');
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    })
  }
}
