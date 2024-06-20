import { Component, OnInit, ViewChild } from '@angular/core';
import { ListingDataService } from '../../../common/services/listing-data.service';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { ListingTableRow } from '../../../common/models/listing-table-row';
import { CommonModule } from '@angular/common';
import { Table, TableModule } from 'primeng/table';
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
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SelectedListingsStateService } from '../../../common/services/selected-listings-state.service';
import { environment } from '../../../../environments/environment';
import { TooltipModule } from 'primeng/tooltip';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

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
    RouterModule,
    TooltipModule,
    ListingDetailsComponent,
  ],
  templateUrl: './listings-table.component.html',
  styleUrl: './listings-table.component.scss'
})
export class ListingsTableComponent implements OnInit {
  @ViewChild('listingsTableMain') listingsTableMain!: Table;

  selectedListings = []
  listings = new Array<ListingTableRow>();
  sort!: { prop: string, dir: 'asc' | 'desc' }
  currentPage!: PagingResponsePageInfo;
  searchTerm!: string;
  searchColumn: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicense' = 'all';
  searchColumns = new Array<DropdownOption>();
  isCEU = false;
  isLegendShown = false;

  readonly addressLowScore = environment.ADDRESS_SCORE;

  constructor(
    private listingService: ListingDataService,
    private userService: UserDataService,
    private router: Router,
    private searchStateService: SelectedListingsStateService,
    private route: ActivatedRoute,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    let page = 1;
    this.searchColumns = [
      { label: 'All', value: 'all' },
      { label: 'Address', value: 'address' },
      { label: 'Listing Url', value: 'url' },
      { label: 'Listing ID', value: 'listingId' },
      { label: 'Host Name', value: 'hostName' },
      { label: 'Business Licence', value: 'businessLicense' },
    ]

    this.route.queryParams.subscribe({
      next: (prms) => {

        if (prms['pageNumber']) {
          page = Number(prms['pageNumber']);
        }
        if (prms['pageSize']) {
          if (!this.currentPage) {
            this.currentPage = {};
          }
          this.currentPage.pageSize = Number(prms['pageSize']);
        }
        if (prms['searchBy']) {
          this.searchColumn = prms['searchBy'];
        }
        if (!this.sort) {
          this.sort = { dir: 'asc', prop: '' }
        }
        if (prms['sortDirection']) {
          this.sort.dir = prms['sortDirection'];
        }
        if (prms['sortColumn']) {
          this.sort.prop = prms['sortColumn'];
        }
        if (prms['searchTerm']) {
          this.searchTerm = prms['searchTerm'];
        }
        this.cloakParams();

        this.userService.getCurrentUser().subscribe({
          next: (currentUser: User) => {
            this.isCEU = currentUser.permissions.includes(ceu_action);
            this.getListings(page);
          },
        });
      }
    });
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

  unselectAll(): void {
    this.selectedListings = []
  }

  onDetailsOpen(row: ListingTableRow): void {
    this.router.navigateByUrl(`/listings/${row.rentalListingId}`);
  }

  onNoticeOpen(): void {
    this.searchStateService.selectedListings = this.selectedListings;
    this.router.navigate(['/bulk-compliance-notice'], { queryParams: { returnUrl: this.getUrlFromState() } })
  }

  onTakedownOpen(): void {
    this.searchStateService.selectedListings = this.selectedListings;
    this.router.navigate(['/bulk-takedown-request'], { queryParams: { returnUrl: this.getUrlFromState() } })
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

  private cloakParams(): void {
    var newURL = location.href.split("?")[0];
    window.history.pushState('object', document.title, newURL);
  }

  private getUrlFromState(): string {
    const state = {
      pageNumber: this.currentPage?.pageNumber || 0,
      pageSize: this.currentPage?.pageSize || 25,
      searchTerm: this.searchTerm || '',
      sortColumn: this.sort?.prop,
      searchBy: this.searchColumn,
      sortDirection: this.sort?.dir || 'asc'
    }

    let url = '/listings?'
    Object.keys(state).forEach((key: string, index: number, array: string[]) => {
      if ((state as any)[key]) {
        url += `${key}=${(state as any)[key]}${index + 1 == array.length ? '' : '&'}`;
      }
    })
    return url;
  }

  private getListings(selectedPageNumber: number = 1): void {
    this.loaderService.loadingStart();
    const searchReq = {} as ListingSearchRequest;
    searchReq[this.searchColumn] = this.searchTerm;

    this.listingService.getListings(
      selectedPageNumber ?? (this.currentPage?.pageNumber || 0),
      this.currentPage?.pageSize || 25,
      this.sort?.prop || '',
      this.sort?.dir || 'asc',
      searchReq).subscribe({
        next: (res: PagingResponse<ListingTableRow>) => {
          this.currentPage = res.pageInfo;
          this.listings = res.sourceList;
        },
        complete: () => {
          this.loaderService.loadingEnd();
        }
      });
  }
}
