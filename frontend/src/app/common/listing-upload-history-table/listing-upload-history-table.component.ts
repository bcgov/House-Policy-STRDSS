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
import { UserDataService } from '../services/user-data.service';
import { forkJoin } from 'rxjs';
import { User } from '../models/user';
import { GlobalLoaderService } from '../services/global-loader.service';

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
  currentUser!: User;

  currentPage!: PagingResponsePageInfo;

  constructor(
    private listingDataService: ListingDataService,
    private delistingService: DelistingService,
    private userDataService: UserDataService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {

    const getCurrentUser = this.userDataService.getCurrentUser()
    const getPlatforms = this.delistingService.getPlatforms();

    forkJoin([getCurrentUser, getPlatforms]).subscribe({
      next: ([currentUser, platforms]) => {
        this.currentUser = currentUser;
        if (currentUser.organizationType !== "Platform") {
          const options: Array<DropdownOption> = [{ label: 'All', value: 0 }, ...platforms]
          this.platformOptions = options;
        }
      }
    });

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

  onDownloadErrors(rowId: number, platform: string, date: string): void {
    this.loaderService.loadingStart(' It may take several minutes to prepare your download file. Please do not close this tab until your download is complete.');
    this.listingDataService.getUploadHistoryErrors(rowId).subscribe({
      next: (content) => {
        const element = document.createElement('a');
        element.setAttribute('href', `data:text/plain;charset=utf-8,${encodeURIComponent(content)}`);
        element.setAttribute('download', `errors_${platform}_${date}.csv`);

        element.click();
        this.loaderService.loadingEnd();
      }
    })
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
