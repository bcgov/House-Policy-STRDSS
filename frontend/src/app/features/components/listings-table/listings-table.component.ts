import { Component, OnInit } from '@angular/core';
import { ListingDataService } from '../../../common/services/listing-data.service';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { ListingTableRow } from '../../../common/models/listing-table-row';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { PaginatorModule } from 'primeng/paginator';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { UserDataService } from '../../../common/services/user-data.service';
import { User } from '../../../common/models/user';
import { ceu_action } from '../../../common/consts/permissions.const';
import { ListingDetailsComponent } from './listing-details/listing-details.component';
import { ListingSearchRequest } from '../../../common/models/listing-search-request';

@Component({
  selector: 'app-listings-table',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DropdownModule,
    CheckboxModule,
    PaginatorModule,
    DialogModule,
    InputTextModule,
    PanelModule,
    ListingDetailsComponent
  ],
  templateUrl: './listings-table.component.html',
  styleUrl: './listings-table.component.scss'
})
export class ListingsTableComponent implements OnInit {
  selectedListings = []
  listings = new Array<ListingTableRow>();
  sort!: { prop: string, dir: 'asc' | 'desc' }
  currentPage!: PagingResponsePageInfo;
  searchTerm!: string;
  searchColumn: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicense' = 'all';
  searchColumns = new Array<DropdownOption>();
  isCEU = false;
  isLegendShown = false;
  isDetailsShown = false;
  selectedListing!: ListingTableRow | null;
  // MOCK: 
  isNotImplemented = true;

  constructor(private listingService: ListingDataService, private userService: UserDataService) { }

  ngOnInit(): void {
    this.searchColumns = [
      { label: 'All', value: 'all' },
      { label: 'Address', value: 'address' },
      { label: 'Listing Url', value: 'url' },
      { label: 'Listing ID', value: 'listingId' },
      { label: 'Host Name', value: 'hostName' },
      { label: 'Business License', value: 'businessLicense' },
    ]

    this.userService.getCurrentUser().subscribe({
      next: (currentUser: User) => {
        this.isCEU = currentUser.permissions.includes(ceu_action);
        this.getListings(1);
      }
    })
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

    this.getListings(this.currentPage.pageNumber);
  }

  onDetailsOpen(row: ListingTableRow): void {
    this.isDetailsShown = true;
    this.selectedListing = row;
  }

  onDetailsClose(reason: 'close' | 'back'): void {
    this.isDetailsShown = false;
    this.selectedListing = null;
  }

  onPageChange(value: any): void {
    this.currentPage.pageSize = value.rows;
    this.currentPage.pageNumber = value.page + 1;

    this.getListings(this.currentPage.pageNumber);
  }

  showLegend(): void {
    this.isLegendShown = true;
  }

  onSearch(): void {
    this.getListings(this.currentPage.pageNumber)
  }

  private getListings(selectedPageNumber: number = 1): void {
    const searchReq = {} as ListingSearchRequest;
    searchReq[this.searchColumn] = this.searchTerm;

    this.listingService.getListings(selectedPageNumber ?? (this.currentPage?.pageNumber || 0), this.currentPage?.pageSize || 25, this.sort?.prop || '', this.sort?.dir || 'asc', searchReq).subscribe({
      next: (res: PagingResponse<ListingTableRow>) => {
        this.currentPage = res.pageInfo;
        this.listings = res.sourceList;
      }
    });
  }
}
