import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AccordionModule } from 'primeng/accordion';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SidebarModule } from 'primeng/sidebar';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ListingFilter } from '../../../../common/models/listing-filter';
import { environment } from '../../../../../environments/environment';
import { PagingResponsePageInfo } from '../../../../common/models/paging-response';
import {
    DropdownOption,
    DropdownOptionOrganization,
} from '../../../../common/models/dropdown-option';
import {
    AggregatedListingTableRow,
    ListingTableRow,
} from '../../../../common/models/listing-table-row';
import { ListingDataService } from '../../../../common/services/listing-data.service';
import { FilterPersistenceService } from '../../../../common/services/filter-persistence.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { SelectedListingsStateService } from '../../../../common/services/selected-listings-state.service';
import { UserDataService } from '../../../../common/services/user-data.service';
import { User } from '../../../../common/models/user';
import { ListingSearchRequest } from '../../../../common/models/listing-search-request';
import { ListingDetails } from '../../../../common/models/listing-details';
import { OrganizationService } from '../../../../common/services/organization.service';
import { UrlProtocolPipe } from '../../../../common/pipes/url-protocol.pipe';
import { forkJoin, tap } from 'rxjs';

@Component({
    selector: 'app-aggregated-listings-table',
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
        TagModule,
        SidebarModule,
        AccordionModule,
        RadioButtonModule,
        UrlProtocolPipe,
    ],
    templateUrl: './aggregated-listings-table.component.html',
    styleUrl: './aggregated-listings-table.component.scss',
})
export class AggregatedListingsTableComponent implements OnInit {
    @ViewChild('paginator') paginator!: Paginator;

    selectedListings: { [key: string]: ListingTableRow } = {};
    aggregatedListings = new Array<AggregatedListingTableRow>();
    sort!: { prop: string; dir: 'asc' | 'desc' };
    currentPage!: PagingResponsePageInfo;
    searchTerm!: string;
    searchColumn: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicence' = 'all';
    searchColumns = new Array<DropdownOption>();
    communities = new Array<DropdownOptionOrganization>();
    groupedCommunities = new Array();
    hosts = new Array<{ primaryHostNm: string, hasMultipleProperties: boolean }>();

    isCEU = false;
    isLegendShown = false;
    isFilterOpened = false;
    currentFilter!: ListingFilter;
    cancelableFilter!: ListingFilter;
    readonly addressLowScore = Number.parseInt(environment.ADDRESS_SCORE);

    get listingsSelected(): number {
        return Object.keys(this.selectedListings).length;
    }

    constructor(
        private listingService: ListingDataService,
        private userService: UserDataService,
        private router: Router,
        private organizationsService: OrganizationService,
        private searchStateService: SelectedListingsStateService,
        private route: ActivatedRoute,
        private loaderService: GlobalLoaderService,
        private cd: ChangeDetectorRef,
        private filterPersistenceService: FilterPersistenceService,
    ) { }

    ngOnInit(): void {
        this.getOrganizations();
        this.initFilters();
        let page = 1;
        this.searchColumns = [
            { label: 'All', value: 'all' },
            { label: 'Address', value: 'address' },
            { label: 'Listing Url', value: 'url' },
            { label: 'Listing ID', value: 'listingId' },
            { label: 'Host Name', value: 'hostName' },
            { label: 'Business Licence', value: 'businessLicence' },
        ];

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
                    this.sort = { dir: 'asc', prop: '' };
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
                if (prms['hostName']) {
                    this.searchColumn = 'hostName';
                    this.searchTerm = prms['hostName'];
                }
                this.cd.detectChanges();
                this.cloakParams();

                this.userService.getCurrentUser().subscribe({
                    next: (currentUser: User) => {
                        this.isCEU = currentUser.organizationType === 'BCGov';
                        this.getListings(page);
                    },
                });
            },
        });
    }

    onGroupRowSelected(e: any, group: AggregatedListingTableRow): void {
        if (e.checked) {
            group.listings.forEach((l) => {
                l.selected = true;
                this.selectedListings[l.rentalListingId] = l;
            });
        } else {
            group.listings.forEach((l) => {
                l.selected = false;
                delete this.selectedListings[l.rentalListingId];
            });
        }
    }

    onListingRowSelected(e: any, listing: ListingTableRow): void {
        const listingGroup = this.aggregatedListings.find((g) =>
            g.listings.some((l) => l.rentalListingId === listing.rentalListingId),
        );

        if (listingGroup) {
            if (e.checked) {
                this.selectedListings[listing.rentalListingId] = listing;
                if (listingGroup.listings.every((l) => l.selected)) {
                    listingGroup.selected = true;
                }
            } else {
                listingGroup.selected = false;
                delete this.selectedListings[listing.rentalListingId];
            }
        }
    }

    onMultihostClicked(group: AggregatedListingTableRow) {
        this.clearFilters();
        this.searchColumn = 'hostName';
        this.searchTerm = group.primaryHostNm;
        this.onSearch();
    }

    onSort(property: string): void {
        if (this.sort) {
            if (this.sort.prop === property) {
                this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
            } else {
                this.sort.prop = property;
                this.sort.dir = 'asc';
            }
        } else {
            this.sort = { prop: property, dir: 'asc' };
        }

        this.paginator.changePage(0);
    }

    unselectAll(): void {
        const listings = Object.values(this.selectedListings);
        listings.forEach((listing) => {
            const listingGroup = this.aggregatedListings.find((g) =>
                g.listings.some((l) => l.rentalListingId === listing.rentalListingId),
            );
            if (listingGroup) {
                listingGroup.selected = false;
            }
            listing.selected = false;
        });

        this.selectedListings = {};
    }

    onDetailsOpen(row: ListingTableRow): void {
        this.router.navigate([`/listings/${row.rentalListingId}`], {
            queryParams: { returnUrl: this.getUrlFromState() },
        });
    }

    onNoticeOpen(): void {
        this.searchStateService.selectedListings = Object.values(
            this.selectedListings,
        ) as unknown as Array<ListingDetails>;
        this.router.navigate(['/bulk-compliance-notice'], {
            queryParams: { returnUrl: this.getUrlFromState() },
        });
    }

    onTakedownOpen(): void {
        this.searchStateService.selectedListings = Object.values(
            this.selectedListings,
        ) as unknown as Array<ListingDetails>;
        this.router.navigate(['/bulk-takedown-request'], {
            queryParams: { returnUrl: this.getUrlFromState() },
        });
    }

    onContactHost(): void {
        this.searchStateService.selectedListings = Object.values(
            this.selectedListings,
        ) as unknown as Array<ListingDetails>;
        this.router.navigate(['/send-compliance-order'], {
            queryParams: { returnUrl: this.getUrlFromState() },
        });
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
        this.getListings(0);
        this.paginator.changePage(0);
    }

    clearSearchBy(_event: any): void {
        this.searchColumn = 'all';
        this.cd.detectChanges();
    }

    get isFilterSet(): boolean {
        if (this.currentFilter === null || this.currentFilter === undefined) {
            return false;
        }

        const byCommunity = !!this.currentFilter.community;
        const byStatus = Object.values(this.currentFilter.byStatus).some((x) => x === true);
        const byLocation = Object.values(this.currentFilter.byLocation).some((x) => x !== '');

        return byStatus || byLocation || byCommunity;
    }

    get isCancelableFilterSet(): boolean {
        if (this.cancelableFilter === null || this.cancelableFilter === undefined) {
            return false;
        }

        const byCommunity = !!this.cancelableFilter.community;
        const byStatus = Object.values(this.cancelableFilter.byStatus).some((x) => x === true);
        const byLocation = Object.values(this.cancelableFilter.byLocation).some((x) => x !== '');

        return byStatus || byLocation || byCommunity;
    }

    openFilterSidebar(): void {
        this.isFilterOpened = true;
        this.cancelableFilter.byLocation = Object.assign({}, this.currentFilter.byLocation);
        this.cancelableFilter.byStatus = Object.assign({}, this.currentFilter.byStatus);
        this.cancelableFilter.community = this.currentFilter.community;
    }

    onClearSearchBox(): void {
        this.searchTerm = '';
    }

    onClearFilters(): void {
        this.clearFilters();
        this.onSearch();
    }

    onCancelFilters(): void {
        this.cancelableFilter = {
            byLocation: {
                isBusinessLicenceRequired: '',
                isPrincipalResidenceRequired: '',
            },
            community: 0,
            byStatus: {},
        };
        this.filterPersistenceService.listingFilter = {
            byLocation: {
                isBusinessLicenceRequired: '',
                isPrincipalResidenceRequired: '',
            },
            community: 0,
            byStatus: {},
        };
        this.isFilterOpened = false;
    }

    onSubmitFilters(): void {
        this.currentFilter.byLocation = Object.assign({}, this.cancelableFilter.byLocation);
        this.currentFilter.byStatus = Object.assign({}, this.cancelableFilter.byStatus);
        this.currentFilter.community = this.cancelableFilter.community;

        if (!this.filterPersistenceService.listingFilter) {
            this.filterPersistenceService.listingFilter = {
                byLocation: {
                    isBusinessLicenceRequired: '',
                    isPrincipalResidenceRequired: '',
                },
                community: 0,
                byStatus: {},
            };
        }

        this.filterPersistenceService.listingFilter.byLocation = Object.assign(
            {},
            this.cancelableFilter.byLocation,
        );
        this.filterPersistenceService.listingFilter.byStatus = Object.assign(
            {},
            this.cancelableFilter.byStatus,
        );
        this.filterPersistenceService.listingFilter.community = this.cancelableFilter.community;

        this.isFilterOpened = false;
        this.onSearch();
    }

    private clearFilters(): void {
        this.filterPersistenceService.listingFilter = {
            byLocation: {
                isBusinessLicenceRequired: '',
                isPrincipalResidenceRequired: '',
            },
            community: 0,
            byStatus: {},
        };

        this.initFilters();
        this.isFilterOpened = false;
    }

    private getListings(selectedPageNumber: number = 1): void {
        this.loaderService.loadingStart();

        const searchReq = {} as ListingSearchRequest;
        searchReq[this.searchColumn] = this.searchTerm;

        this.listingService
            .getAggregatedListings(
                selectedPageNumber ?? (this.currentPage?.pageNumber || 0),
                this.currentPage?.pageSize || 25,
                this.sort?.prop || '',
                this.sort?.dir || 'asc',
                searchReq,
                this.currentFilter,
            ).pipe(
                tap(res => { this.calculateIfHostsHaveMoreThanOneProperty(res.listings.sourceList); })
            )
            .subscribe({
                next: (res) => {
                    this.currentPage = res.listings.pageInfo;
                    this.currentPage.totalCount = res.count;
                    this.aggregatedListings = res.listings.sourceList.map(
                        (l: AggregatedListingTableRow, index: number) => {
                            return {
                                ...l,
                                id:
                                    l.effectiveHostNm +
                                    l.matchAddressTxt +
                                    l.effectiveBusinessLicenceNo +
                                    index,
                            };
                        },
                    );

                    this.restoreSelectedListings();
                },
                complete: () => {
                    this.loaderService.loadingEnd();
                    this.cd.detectChanges();
                },
            });
    }

    private cloakParams(): void {
        var newURL = location.href.split('?')[0];
        window.history.pushState('object', document.title, newURL);
    }

    private initFilters(): void {
        if (this.filterPersistenceService.listingFilter) {
            this.currentFilter = {
                byLocation: this.filterPersistenceService.listingFilter.byLocation,
                community: this.filterPersistenceService.listingFilter.community,
                byStatus: this.filterPersistenceService.listingFilter.byStatus,
            };
            this.cancelableFilter = {
                byLocation: this.filterPersistenceService.listingFilter.byLocation,
                community: this.filterPersistenceService.listingFilter.community,
                byStatus: this.filterPersistenceService.listingFilter.byStatus,
            };
        } else {
            this.currentFilter = {
                byLocation: {
                    isBusinessLicenceRequired: '',
                    isPrincipalResidenceRequired: '',
                },
                community: 0,
                byStatus: {},
            };
            this.cancelableFilter = {
                byLocation: {
                    isBusinessLicenceRequired: '',
                    isPrincipalResidenceRequired: '',
                },
                community: 0,
                byStatus: {},
            };
        }

        this.cd.detectChanges();
    }

    private calculateIfHostsHaveMoreThanOneProperty(listings: Array<AggregatedListingTableRow>): void {
        const uniqueObjects = new Map<string, AggregatedListingTableRow>();

        listings.forEach(obj => {
            uniqueObjects.set(obj.primaryHostNm, obj);
        });

        const hosts = Array.from(uniqueObjects.values()).map(x => ({ primaryHostNm: x.primaryHostNm, hasMultipleProperties: false }));

        forkJoin(hosts.map(h => this.listingService.getHostListingsCount(h.primaryHostNm)))
            .subscribe(hostCount => {
                this.aggregatedListings.forEach(al => {
                    al.hasMultipleProperties = hostCount.find(x => x.primaryHostNm === al.primaryHostNm)?.hasMultipleProperties ?? false;
                    this.cd.detectChanges();
                })
            });
    }

    private getUrlFromState(): string {
        const state = {
            pageNumber: this.currentPage?.pageNumber || 0,
            pageSize: this.currentPage?.pageSize || 25,
            searchTerm: this.searchTerm || '',
            sortColumn: this.sort?.prop,
            searchBy: this.searchColumn,
            sortDirection: this.sort?.dir || 'asc',
        };

        let url = '/aggregated-listings?';
        Object.keys(state).forEach((key: string, index: number, array: string[]) => {
            if ((state as any)[key]) {
                url += `${key}=${(state as any)[key]}${index + 1 == array.length ? '' : '&'}`;
            }
        });
        return url;
    }

    private restoreSelectedListings(): void {
        if (!!this.selectedListings) {
            Object.values(this.selectedListings).forEach((value) => {
                const listingGroup = this.aggregatedListings.find((g) =>
                    g.listings.some((l) => l.rentalListingId === value.rentalListingId),
                );
                const listing = listingGroup?.listings.find(
                    (x) => x.rentalListingId === value.rentalListingId,
                );

                if (listing) {
                    listing.selected = true;
                }

                if (listingGroup) {
                    if (listingGroup.listings.every((l) => l.selected)) {
                        listingGroup.selected = true;
                    }
                }
            });
        }
    }

    private getOrganizations(): void {
        this.organizationsService.getOrganizations('LG').subscribe({
            next: (orgs) => {
                this.communities = orgs.map((org: DropdownOptionOrganization) => ({
                    label: org.label,
                    value: org.value,
                    localGovernmentType: org.localGovernmentType || 'Other',
                }));

                const groupedData: Array<any> = this.communities.reduce((acc: any, curr: any) => {
                    const existingGroup = acc.find(
                        (group: any) => group.value === curr.localGovernmentType,
                    );
                    if (existingGroup) {
                        existingGroup.items.push({ label: curr.label, value: curr.value });
                    } else {
                        acc.push({
                            label: curr.localGovernmentType,
                            value: curr.localGovernmentType,
                            items: [{ label: curr.label, value: curr.value }],
                        });
                    }

                    return acc;
                }, []);
                const municipality = groupedData.filter((x) => x.label === 'Municipality')[0];
                const regional = groupedData.filter(
                    (x) => x.label === 'Regional District Electoral Area',
                )[0];
                const other = groupedData.filter((x) => x.label === 'Other')[0];
                const firstNations = groupedData.filter(
                    (x) => x.label === 'First Nations Community',
                )[0];
                const uncategorized = groupedData.filter(
                    (x) =>
                        x.label !== 'Municipality' &&
                        x.label !== 'Regional District Electoral Area' &&
                        x.label !== 'Other' &&
                        x.label !== 'First Nations Community',
                );

                const sorted = [];

                if (municipality) sorted.push(municipality);
                if (regional) sorted.push(regional);
                if (other) sorted.push(other);
                if (firstNations) sorted.push(firstNations);
                if (uncategorized.length) sorted.push(...uncategorized);

                this.groupedCommunities = sorted;
            },
        });
    }
}
