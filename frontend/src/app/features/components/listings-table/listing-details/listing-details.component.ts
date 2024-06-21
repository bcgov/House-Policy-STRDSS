import { Component, OnInit } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ListingDataService } from '../../../../common/services/listing-data.service';
import { ListingDetails } from '../../../../common/models/listing-details';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { UserDataService } from '../../../../common/services/user-data.service';
import { environment } from '../../../../../environments/environment';
import { SelectedListingsStateService } from '../../../../common/services/selected-listings-state.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';

@Component({
  selector: 'app-listing-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    PanelModule,
    TableModule,
    DialogModule,
    TooltipModule,
  ],
  templateUrl: './listing-details.component.html',
  styleUrl: './listing-details.component.scss'
})
export class ListingDetailsComponent implements OnInit {
  id!: number;
  listing!: ListingDetails;
  isLegendShown = false;
  addressWarningScoreLimit = Number.parseInt(environment.ADDRESS_SCORE);
  isCEU = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private listingService: ListingDataService,
    private userDataService: UserDataService,
    private searchStateService: SelectedListingsStateService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.id = this.route.snapshot.params['id'];
    this.userDataService.getCurrentUser().subscribe({
      next: (user) => {
        this.isCEU = user.permissions.includes('ceu_action');
      }, complete: () => {
        this.loaderService.loadingEnd();
      },
    });

    this.getListingDetailsById(this.id);
  }

  showLegend(): void {
    this.isLegendShown = true;
  }

  sendTakedownRequest(): void {
    this.searchStateService.selectedListings = [this.listing];
    this.router.navigate(['/bulk-takedown-request'], { queryParams: { returnUrl: this.getUrlFromState() } })
  }

  sendNoticeOfNonCompliance(): void {
    this.searchStateService.selectedListings = [this.listing];
    this.router.navigate(['/bulk-compliance-notice'], { queryParams: { returnUrl: this.getUrlFromState() } })
  }

  private getUrlFromState(): string {
    return `/listing/${this.id}`
  }

  private getListingDetailsById(id: number): void {
    this.loaderService.loadingStart();
    this.listingService.getListingDetailsById(id).subscribe({
      next: (response: ListingDetails) => {
        this.listing = response;
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }
}
