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
import { PagingResponse, PagingResponsePageInfo } from '../models/paging-response';

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
  selectedPlatformId = 0;
  sort!: { prop: string, dir: 'asc' | 'desc' }

  currentPage!: PagingResponsePageInfo;

  constructor(
    private listingDataService: ListingDataService,
    private delistingService: DelistingService,
  ) { }

  ngOnInit(): void {
    this.delistingService.getPlatforms().subscribe((platformOptions) => {
      const options: Array<DropdownOption> = [{ label: 'All', value: 0 }, ...platformOptions]
      this.platformOptions = options;
    });

    this.getHistoryUploadRecords(1);
  }

  onPlatformSelected(_value: number): void {
    this.getHistoryUploadRecords(1);
  }

  onSort(property: string): void {
    if (this.sort) {
      if (this.sort.prop === property) {
        this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
      }
      else {
        this.sort.prop = property;
        this.sort.dir = 'asc';
      }
    }
    else {
      this.sort = { prop: property, dir: 'asc' };
    }

    this.getHistoryUploadRecords(this.currentPage.pageNumber);
  }

  onPageChange(value: any): void {
    this.getHistoryUploadRecords(value.page + 1);
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

  private getHistoryUploadRecords(selectedPageNumber?: number): void {
    this.listingDataService.getListingUploadHistoryRecords(selectedPageNumber ?? (this.currentPage?.pageNumber || 0), this.currentPage?.pageSize || 10, this.selectedPlatformId, this.sort?.prop || '', this.sort?.dir || 'asc').subscribe({
      next: (value) => {
        this.processRecords(value);
      },
    })
  }

  private processRecords(raw: PagingResponse<ListingUploadHistoryRecord>): void {
    this.currentPage = raw.pageInfo;
    this.listings = this.extendData(this.isSmall ? raw.sourceList.slice(0, 3) : raw.sourceList);
  }

  private extendData(data: Array<ListingUploadHistoryRecord>): Array<ListingUploadHistoryRecord> {
    return data.map((record) => {
      record.uploadedBy = `${record.familyNm}, ${record.givenNm}`;

      return record;
    });
  }
}
