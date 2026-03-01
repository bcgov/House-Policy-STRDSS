import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ListingDataService } from '../../../../common/services/listing-data.service';
import { ListingAddressCandidate, ListingDetails } from '../../../../common/models/listing-details';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { UserDataService } from '../../../../common/services/user-data.service';
import { environment } from '../../../../../environments/environment';
import { SelectedListingsStateService } from '../../../../common/services/selected-listings-state.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { address_write } from '../../../../common/consts/permissions.const';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { TagModule } from 'primeng/tag';
import { BusinessLicence } from '../../../../common/models/business-licence';
import { BusinessLicenceService } from '../../../../common/services/business-licence.service';
import { BLSearchResultRow } from '../../../../common/models/bl-search-result-row';
import { UrlProtocolPipe } from '../../../../common/pipes/url-protocol.pipe';
import { TextCleanupPipe } from '../../../../common/pipes/text-cleanup.pipe';
import { NotReportedPipe } from '../../../../common/pipes/not-reported.pipe';
import { tap } from 'rxjs';

@Component({
  selector: 'app-listing-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    RadioButtonModule,
    PanelModule,
    TableModule,
    DialogModule,
    TooltipModule,
    TagModule,
    UrlProtocolPipe,
    TextCleanupPipe,
    NotReportedPipe,
    RouterModule,
  ],
  templateUrl: './listing-details.component.html',
  styleUrl: './listing-details.component.scss'
})
export class ListingDetailsComponent implements OnInit {
  id!: number;
  listing!: ListingDetails;
  isLegendShown = false;
  isEditAddressShown = false;
  isMatchBlShown = false;
  isUnlinkBlShow = false;
  addressWarningScoreLimit = Number.parseInt(environment.ADDRESS_SCORE);
  isCEU = false;
  orgId!: number;

  canUserEditAddress = false;
  confirmTheBestMatchAddress = false;
  addressChangeCandidates = new Array<ListingAddressCandidate>();
  selectedCandidate!: ListingAddressCandidate;
  isJurisdictionDifferent = false;
  blInfo!: BusinessLicence;
  blSearchResults = new Array<BLSearchResultRow>();
  selectedBl!: BLSearchResultRow | null;
  searchBlText = '';
  noBlsFound = false;
  returnUrl!: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private listingService: ListingDataService,
    private userDataService: UserDataService,
    private blService: BusinessLicenceService,
    private searchStateService: SelectedListingsStateService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
    private errorService: ErrorHandlingService,
  ) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.id = this.route.snapshot.params['id'];

    this.route.queryParams.subscribe(
      (param) => { this.returnUrl = param['returnUrl']; });

    this.userDataService.getCurrentUser().subscribe({
      next: (user) => {
        this.isCEU = user.organizationType === 'BCGov';
        this.orgId = user.organizationId;
        this.canUserEditAddress = user.permissions.includes(address_write);
      }, complete: () => {
        this.loaderService.loadingEnd();
      },
    });

    this.getListingDetailsById(this.id);
  }

  get isSearchBlDisabled(): boolean {
    return !this.searchBlText.trim();
  }

  /** True when listing status is D (deactivated due to platform data issue). */
  get isDeactivated(): boolean {
    return this.listing?.listingStatusType === 'D';
  }

  /**
   * Returns true if any month in listing history has nights stayed greater than
   * the number of calendar days in that month (indicates multiple units under one listing).
   */
  get hasNightsExceedingDaysInAnyMonth(): boolean {
    if (!this.listing?.listingHistory?.length) {
      return false;
    }
    return this.listing.listingHistory.some(
      (h) =>
        typeof h.nightsBookedQty === 'number' &&
        h.nightsBookedQty !== -1 &&
        h.nightsBookedQty > this.getDaysInMonth(h.reportPeriodYM)
    );
  }

  /** Returns the number of calendar days in the given YYYY-MM period. */
  getDaysInMonth(periodYm: string): number {
    const match = /^(\d{4})-(\d{2})$/.exec(periodYm);
    if (!match) {
      return 0;
    }
    const year = parseInt(match[1], 10);
    const month = parseInt(match[2], 10); // 1-12
    return new Date(year, month, 0).getDate();
  }

  isBoolDefined(prop: boolean | undefined): boolean {
    return typeof prop === 'boolean';
  }

  showLegend(): void {
    this.isLegendShown = true;
  }

  showBlMatchPopup(): void {
    this.isMatchBlShown = true;
  }

  showBlUnlinkPopup(): void {
    this.isUnlinkBlShow = true;
  }

  onSearchBl(): void {
    this.loaderService.loadingStart();

    this.blService.searchBls(this.orgId, this.searchBlText.trim()).subscribe({
      next: (result) => {
        this.blSearchResults = result;
        this.noBlsFound = !result.length;
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  onUpdateBl(): void {
    if (this.selectedBl) {
      this.loaderService.loadingStart();
      this.blService.linkBl(this.listing.rentalListingId, this.selectedBl.businessLicenceId).subscribe({
        next: () => {
          this.errorService.showSuccess('The Business Licence was Successfully Updated.');
          this.cleanupBl();
          this.isMatchBlShown = false;
          this.getListingDetailsById(this.id);
        },
        complete: () => {
          this.loaderService.loadingEnd();
        }
      });
    }
  }

  onUnlinkBl(): void {
    this.loaderService.loadingStart();
    this.blService.unLinkBl(this.listing.rentalListingId).subscribe({
      next: () => {
        this.errorService.showSuccess('The Business Licence was Successfully Unlinked.');
        this.cleanupBl();
        this.isUnlinkBlShow = false;
        this.getListingDetailsById(this.id);
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  onCancelUpdateBl(): void {
    this.isMatchBlShown = false;
  }

  onCancelUnlinkBl(): void {
    this.isUnlinkBlShow = false;
  }

  sendTakedownRequest(): void {
    this.searchStateService.selectedListings = [this.listing];
    this.router.navigate(['/bulk-takedown-request'], { queryParams: { returnUrl: this.getUrlFromState() } });
  }

  sendNoticeOfNonCompliance(): void {
    this.searchStateService.selectedListings = [this.listing];
    this.router.navigate(['/bulk-compliance-notice'], { queryParams: { returnUrl: this.getUrlFromState() } });
  }

  onContactHost(): void {
    this.searchStateService.selectedListings = [this.listing];
    this.router.navigate(['/send-compliance-order'], { queryParams: { returnUrl: this.getUrlFromState() } });
  }

  onAddressChangeClicked(): void {
    this.isEditAddressShown = true;
  }

  onConfirmFlagChanged(value: any): void {
    if (value) {
      this.addressChangeCandidates = [];
      this.selectedCandidate = {
        address: '', organizationId: 0, score: 0
      };
      this.isJurisdictionDifferent = false;
    }
  }

  onSearchCandidates(value: string): void {
    this.listingService.getAddressCandidates(value).subscribe({
      next:
        (candidates) => {
          this.addressChangeCandidates = candidates;
          this.cd.detectChanges();
        }
    });
  }

  onCandidateSelected(_: any): void {
    this.isJurisdictionDifferent = this.listing.managingOrganizationId !== this.selectedCandidate.organizationId;
  }

  onCancelAddressChange(): void {
    this.isEditAddressShown = false;
    this.isJurisdictionDifferent = false;
    this.selectedCandidate = {
      address: '', organizationId: 0, score: 0
    };
    this.addressChangeCandidates = [];
  }

  navigateToListingsByHost(): void {
    const owner = this.listing.hosts.find(x => x.isPropertyOwner);
    if (owner) {
      const url = this.router.serializeUrl(this.router.createUrlTree([`/aggregated-listings`], { queryParams: { hostName: owner.fullNm } }));
      window.open(url, '_blank');
    }
    else {
      this.errorService.showError(`Unable to retrieve the host's name. Neither host is the owner`);
    }
  }

  onSubmitAddressChange(): void {
    let observableRef;
    this.loaderService.loadingStart();

    if (this.confirmTheBestMatchAddress) {
      observableRef = this.listingService.confirmAddress(this.listing.rentalListingId);
    } else {
      observableRef = this.listingService.changeAddress(this.listing.rentalListingId, this.selectedCandidate.address);
    }

    observableRef.subscribe({
      next: () => {
        this.errorService.showSuccess('The Address was Successfully Updated.');
        if (this.isJurisdictionDifferent) {
          this.loaderService.loadingEnd();
          this.router.navigateByUrl('/listings');
        } else {
          this.getListingDetailsById(this.id);
        }
      }, complete: () => {
        this.onCancelAddressChange();
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  private cleanupBl(): void {
    this.blSearchResults = [];
    this.searchBlText = '';
    this.selectedBl = null;
  }

  private getUrlFromState(): string {
    return `/listing/${this.id}`;
  }

  private getListingDetailsById(id: number): void {
    this.loaderService.loadingStart();
    this.listingService.getListingDetailsById(id)
      .pipe(
        tap((listing) => {
          if (listing.hosts.length) {
            const owner = listing.hosts.find(x => x.isPropertyOwner);

            if (owner) {
              this.listingService.getHostListingsCount(owner.fullNm)
                .subscribe(x => {
                  this.listing.hasMultipleProperties = x.hasMultipleProperties;
                  this.cd.detectChanges();
                });
            }
          }
        })
      ).subscribe({
        next: (response: ListingDetails) => {
          this.listing = response;
          this.blInfo = response.bizLicenceInfo;
        },
        complete: () => {
          this.loaderService.loadingEnd();
          this.cd.detectChanges();
        }
      });
  }
}
