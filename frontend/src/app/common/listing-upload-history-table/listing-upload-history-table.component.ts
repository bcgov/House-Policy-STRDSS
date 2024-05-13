import { Component, Input, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { ListingUploadHistoryRecord } from '../models/listing-upload-history-record';
import { ListingDataService } from '../services/listing-data.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { DropdownOption } from '../models/dropdown-option';
import { DelistingService } from '../services/delisting.service';
import { DropdownModule } from 'primeng/dropdown';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-listing-upload-history-table',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    PaginatorModule,
    DropdownModule,
    CommonModule,
    RouterModule,
    CardModule,
    ReactiveFormsModule,
  ],
  templateUrl: './listing-upload-history-table.component.html',
  styleUrl: './listing-upload-history-table.component.scss'
})
export class ListingUploadHistoryTableComponent implements OnInit {
  @Input() isSmall = false;

  listings = new Array<ListingUploadHistoryRecord>();
  platformOptions = new Array<DropdownOption>();

  selectedPlatformId = -1;

  constructor(
    private listingDataService: ListingDataService,
    private delistingService: DelistingService,
  ) { }

  ngOnInit(): void {
    this.delistingService.getPlatforms().subscribe((platformOptions) => {
      const options: Array<DropdownOption> = [{ label: 'All', value: -1 }, ...platformOptions]
      this.platformOptions = options;
    });

    this.listingDataService.getListingUploadHistoryRecords().subscribe({
      next: (value) => {
        console.log('Listings', value);
        this.listings = this.isSmall ? value.sourceList.slice(0, 3) : value.sourceList;
      },
    })
  }

  onPlatformSelected(value: number): void {
    console.log('onPlatformSelected Value', value);
    // TODO: Perform search
  }

  onPageChange(value: any): void {
    console.log('onPageChange Value', value);
  }

  onDownloadErrors(_rowId: number, platform: string, date: string): void {
    //TODO: Get errors by rowId

    //NOTE: MOCK data. Remove before release
    const content =
      `Header1,Header2,Header3\r\nc1,b1,a4\r\nc2,b2,a5\r\nc3,b3,a6\r\nc4,b4,a7\r\nc5,b5,a8\r\nc6,b6,a9\r\nc7,b7,a10\r\nc8,b8,a11`;
    //NOTE:MOCK end  

    const element = document.createElement('a');
    element.setAttribute('href', `data:text/plain;charset=utf-8,${encodeURIComponent(content)}`);
    element.setAttribute('download', `errors_${platform}_${date}.csv`);

    element.click();
  }
}
