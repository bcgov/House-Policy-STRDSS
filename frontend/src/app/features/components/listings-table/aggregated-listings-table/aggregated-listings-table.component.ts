import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AccordionModule } from 'primeng/accordion';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { RadioButtonModule } from 'primeng/radiobutton';
import { DrawerModule } from 'primeng/drawer';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ListingFilter } from '../../../../common/models/listing-filter';
import { environment } from '../../../../../environments/environment';
import { PagingResponse, PagingResponsePageInfo } from '../../../../common/models/paging-response';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
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
import { VisitedListingsSessionService } from '../../../../common/services/visited-listings-session.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { SelectedListingsStateService } from '../../../../common/services/selected-listings-state.service';
import { UserDataService } from '../../../../common/services/user-data.service';
import { User } from '../../../../common/models/user';
import { ListingSearchRequest } from '../../../../common/models/listing-search-request';
import { ListingDetails } from '../../../../common/models/listing-details';
import { OrganizationService } from '../../../../common/services/organization.service';
import { UrlProtocolPipe } from '../../../../common/pipes/url-protocol.pipe';
import { forkJoin } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-aggregated-listings-table',
    standalone: true,
    imports: [
        CommonModule,
        TableModule,
        ButtonModule,
        SelectModule,
        CheckboxModule,
        PaginatorModule,
        DialogModule,
        InputTextModule,
        PanelModule,
        RouterModule,
        TooltipModule,
        TagModule,
        DrawerModule,
        AccordionModule,
        RadioButtonModule,
        UrlProtocolPipe,
        FormsModule,
        ToggleSwitchModule,
    ],
    templateUrl: './aggregated-listings-table.component.html',
    styleUrl: './aggregated-listings-table.component.scss',
})
export class AggregatedListingsTableComponent implements OnInit {
    @ViewChild('paginator') paginator!: Paginator;
    @ViewChild('table') table!: Table;

    selectedListings: { [key: string]: ListingTableRow } = {};
    displayedListings = new Array<AggregatedListingTableRow>();
    sort!: { prop: string; dir: 'asc' | 'desc' };
    currentPage!: PagingResponsePageInfo;
    searchTerm!: string;
    searchColumn: 'all' | 'address' | 'url' | 'listingId' | 'hostName' | 'businessLicence' | 'registrationNumber' = 'all';
    searchColumns = new Array<DropdownOption>();
    communities = new Array<DropdownOptionOrganization>();
    groupedCommunities = new Array();

    isCEU = false;
    isLegendShown = false;
    isFilterOpened = false;
    currentFilter!: ListingFilter;
    cancelableFilter!: ListingFilter;
    showRecentOnly = true; // Default to "Recently Reported"
    readonly addressLowScore = Number.parseInt(environment.ADDRESS_SCORE);
    
    // Track which rows are currently loading/expanding to prevent multiple clicks
    expandingRows = new Set<string>();
    /** Prevents duplicate getListings calls (e.g. Enter key + button activation). */
    private getListingsInProgress = false;
    /** When true, the next onPageChange came from paginator sync after load/search; skip duplicate fetch. */
    private skipNextPageChange = false;
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
        readonly visitedListings: VisitedListingsSessionService,
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
            { label: 'Registration Number', value: 'registrationNumber' }
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
                    this.sort = { dir: 'desc', prop: 'latestReportPeriodYm' };
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
                        // Initialize currentPage with page from query params
                        if (!this.currentPage) {
                            this.currentPage = {
                                pageNumber: page,
                                pageSize: 25,
                                totalCount: 0,
                                itemCount: 0
                            };
                        } else {
                            this.currentPage.pageNumber = page;
                        }
                        this.getListings();
                    },
                });
            },
        });
    }

    onGroupRowSelected(e: any, group: AggregatedListingTableRow): void {
        if (!group.listings?.length) {
            return;
        }
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
        const listingGroup = this.displayedListings.find((g) =>
            g.listings?.some((l) => l.rentalListingId === listing.rentalListingId),
        );

        if (listingGroup) {
            if (e.checked) {
                this.selectedListings[listing.rentalListingId] = listing;
                if (listingGroup.listings?.every((l) => l.selected)) {
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
        this.searchTerm = group.effectiveHostNm ?? group.primaryHostNm ?? '';
        this.onSearch();
    }

    onRowExpand(event: any): void {
        const row = event.data as AggregatedListingTableRow;
        const rowId = this.getRowId(row);

        if (this.expandingRows.has(rowId)) {
            return;
        }

        if (row.listings?.length > 0) {
            return;
        }

        row.expandLoadError = false;
        this.expandingRows.add(rowId);
        this.cd.detectChanges();

        const searchReq = {} as ListingSearchRequest;
        searchReq[this.searchColumn] = this.searchTerm;

        this.listingService
            .getAggregatedGroupListings(
                searchReq,
                this.currentFilter,
                this.showRecentOnly,
                row.bcRegistryNo,
                row.matchAddressTxt,
                row.matchUnitNo,
                row.effectiveHostNm,
                row.effectiveBusinessLicenceNo,
            )
            .subscribe({
                next: (raw) => {
                    row.listings = raw.map((dto) => this.mapChildListingRow(dto));
                    this.restoreSelectedListingsForGroup(row);
                },
                error: () => {
                    row.expandLoadError = true;
                    row.listings = [];
                },
                complete: () => {
                    this.expandingRows.delete(rowId);
                    this.cd.detectChanges();
                },
            });
    }

    onRowCollapse(event: any): void {
        const row = event.data as AggregatedListingTableRow;
        const rowId = this.getRowId(row);
        this.expandingRows.delete(rowId);
    }

    isRowExpanding(row: AggregatedListingTableRow): boolean {
        const rowId = this.getRowId(row);
        return this.expandingRows.has(rowId);
    }


    private getRowId(row: AggregatedListingTableRow): string {
        return row.id ?? '';
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

        if (this.currentPage) {
            this.currentPage.pageNumber = 1;
        }
        this.skipNextPageChange = true;
        this.getListings();
        if (this.paginator) {
            this.paginator.changePage(0);
            setTimeout(() => {
                this.skipNextPageChange = false;
            }, 0);
        }
    }

    unselectAll(): void {
        const listings = Object.values(this.selectedListings);
        listings.forEach((listing) => {
            const listingGroup = this.displayedListings.find((g) =>
                g.listings?.some((l) => l.rentalListingId === listing.rentalListingId),
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

    onRowClick(event: MouseEvent, listing: ListingTableRow): void {
        // Don't trigger if clicking on checkbox, link, or button
        const target = event.target as HTMLElement;
        if (target.closest('p-checkbox') || 
            target.closest('a') || 
            target.closest('button') ||
            target.closest('input') ||
            target.closest('.multihost-icon')) {
            return;
        }

        this.visitedListings.markVisited(listing.rentalListingId);
        this.cd.detectChanges();

        // Open listing details in a new tab
        const url = this.router.serializeUrl(this.router.createUrlTree(['/listing', listing.rentalListingId]));
        window.open(url, '_blank');
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
        if (!this.currentPage) {
            this.currentPage = {};
        }
        if (this.skipNextPageChange) {
            this.skipNextPageChange = false;
            this.currentPage.pageSize = value.rows;
            this.currentPage.pageNumber = value.page + 1;
            return;
        }
        this.currentPage.pageSize = value.rows;
        this.currentPage.pageNumber = value.page + 1;
        this.getListings();
    }

    showLegend(): void {
        this.isLegendShown = true;
    }

    onSearch(): void {
        if (this.currentPage) {
            this.currentPage.pageNumber = 1;
        }
        this.skipNextPageChange = true;
        this.getListings();
        if (this.paginator) {
            this.paginator.changePage(0);
            setTimeout(() => {
                this.skipNextPageChange = false;
            }, 0);
        }
    }

    onToggleChange(): void {
        this.unselectAll();
        if (this.table) {
            this.table.expandedRowKeys = {};
        }
        if (this.paginator) {
            this.paginator.changePage(0);
        }
        if (this.currentPage) {
            this.currentPage.pageNumber = 1;
        }
        this.getListings();
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
        this.onSearch();
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

    private getListings(): void {
        if (this.getListingsInProgress) {
            return;
        }
        this.getListingsInProgress = true;
        this.loaderService.loadingStart();

        const searchReq = {} as ListingSearchRequest;
        searchReq[this.searchColumn] = this.searchTerm;

        const pageNumber = Math.max(1, this.currentPage?.pageNumber ?? 1);
        const pageSize = this.currentPage?.pageSize ?? 25;
        const orderBy = this.sort?.prop || '';
        const direction = this.sort?.dir || 'asc';

        forkJoin({
            count: this.listingService.getAggregatedListingsCount(
                searchReq,
                this.currentFilter,
                this.showRecentOnly,
            ),
            data: this.listingService.getAggregatedListings(
                pageNumber,
                pageSize,
                orderBy,
                direction,
                searchReq,
                this.currentFilter,
                this.showRecentOnly,
                false,
            ),
        }).subscribe({
            next: ({ count, data }: { count: number; data: PagingResponse<AggregatedListingTableRow> }) => {
                this.currentPage = data.pageInfo;
                this.currentPage.totalCount = count;
                this.displayedListings = (data.sourceList ?? []).map((row) =>
                    this.normalizeAggregatedSummaryRow(row),
                );
                if (this.table) {
                    this.table.expandedRowKeys = {};
                }
                this.restoreSelectedListings();
                if (this.paginator && data.pageInfo.pageNumber === 1) {
                    this.skipNextPageChange = true;
                    this.paginator.changePage(0);
                    setTimeout(() => {
                        this.skipNextPageChange = false;
                    }, 0);
                }
            },
            error: () => {
                this.getListingsInProgress = false;
                this.loaderService.loadingEnd();
                this.cd.detectChanges();
            },
            complete: () => {
                this.getListingsInProgress = false;
                this.loaderService.loadingEnd();
                this.cd.detectChanges();
            },
        });
    }

    private stableGroupRowId(row: AggregatedListingTableRow): string {
        if (row.bcRegistryNo?.trim()) {
            return `reg:${row.bcRegistryNo.trim()}`;
        }
        const delim = '|';
        return `cmp:${encodeURIComponent(row.matchAddressTxt ?? '')}${delim}${encodeURIComponent(row.matchUnitNo ?? '')}${delim}${encodeURIComponent(row.effectiveHostNm ?? '')}${delim}${encodeURIComponent(row.effectiveBusinessLicenceNo ?? '')}`;
    }

    private normalizeAggregatedSummaryRow(row: AggregatedListingTableRow): AggregatedListingTableRow {
        return {
            ...row,
            id: this.stableGroupRowId(row),
            listings: [],
            nightsBookedYtdQty: row.nightsBookedYtdQty ?? 0,
            listingCount: row.listingCount ?? 0,
            hasMultipleProperties: row.hasMultipleProperties ?? false,
            expandLoadError: false,
        };
    }

    private mapChildListingRow(dto: Record<string, unknown>): ListingTableRow {
        const id = Number(dto['rentalListingId']);
        return {
            rentalListingId: id,
            listingStatusType: (dto['listingStatusType'] as string) ?? '',
            offeringOrganizationNm: (dto['offeringOrganizationNm'] as string) ?? '',
            platformListingNo: (dto['platformListingNo'] as string) ?? '',
            originalAddressTxt: (dto['originalAddressTxt'] as string) ?? '',
            matchAddressTxt: (dto['matchAddressTxt'] as string) ?? '',
            isEntireUnit: !!dto['isEntireUnit'],
            nightsBookedYtdQty: Number(dto['nightsBookedYtdQty'] ?? 0),
            businessLicenceNo: (dto['businessLicenceNo'] as string) ?? '',
            businessLicenceNoMatched: (dto['businessLicenceNoMatched'] as string) ?? '',
            lastActionNm: (dto['lastActionNm'] as string) ?? '',
            lastActionDtm: dto['lastActionDtm'] != null ? String(dto['lastActionDtm']) : '',
            isChangedBusinessLicence: !!dto['isChangedBusinessLicence'],
            latestReportPeriodYm:
                dto['latestReportPeriodYm'] != null ? String(dto['latestReportPeriodYm']) : undefined,
            isTakenDown: !!dto['isTakenDown'],
            isLgTransferred: !!dto['isLgTransferred'],
            isMatchVerified: !!dto['isMatchVerified'],
            isMatchCorrected: !!dto['isMatchCorrected'],
            isChangedAddress: !!dto['isChangedAddress'],
            takeDownReason: (dto['takeDownReason'] as string) ?? undefined,
            platformListingUrl: (dto['platformListingUrl'] as string) ?? undefined,
            matchScoreAmt: dto['matchScoreAmt'] as number | undefined,
            bcRegistryNo: (dto['bcRegistryNo'] as string) ?? undefined,
            businessLicenceId: dto['businessLicenceId'] as number | undefined,
            licenceStatusType: dto['licenceStatusType'] as ListingTableRow['licenceStatusType'],
            filtered: true,
            selected: !!this.selectedListings[id]?.selected,
        };
    }

    private restoreSelectedListingsForGroup(group: AggregatedListingTableRow): void {
        if (!group.listings?.length) {
            return;
        }
        group.listings.forEach((l) => {
            if (this.selectedListings[l.rentalListingId]) {
                l.selected = true;
            }
        });
        if (group.listings.every((l) => l.selected)) {
            group.selected = true;
        }
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
        if (!this.selectedListings) {
            return;
        }
        Object.values(this.selectedListings).forEach((value) => {
            const listingGroup = this.displayedListings.find((g) =>
                g.listings?.some((l) => l.rentalListingId === value.rentalListingId),
            );
            const listing = listingGroup?.listings?.find((x) => x.rentalListingId === value.rentalListingId);

            if (listing) {
                listing.selected = true;
            }

            if (listingGroup?.listings?.every((l) => l.selected)) {
                listingGroup.selected = true;
            }
        });
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
