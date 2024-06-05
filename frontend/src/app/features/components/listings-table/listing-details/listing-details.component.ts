import { Component, OnInit } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ListingDataService } from '../../../../common/services/listing-data.service';
import { ListingDetails } from '../../../../common/models/listing-details';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { UserDataService } from '../../../../common/services/user-data.service';

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
  addressWarningScoreLimit = 75;
  isCEU = false;

  constructor(private route: ActivatedRoute, private listingService: ListingDataService, private userDataService: UserDataService) {
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.userDataService.getCurrentUser().subscribe({
      next: (user) => {
        this.isCEU = user.permissions.includes('ceu_action');
      }
    });

    this.getListingDetailsById(this.id);
  }

  showLegend(): void {
    this.isLegendShown = true;
  }

  sendTakedownRequest(): void {

  }

  sendNoticeOfNonCompliance(): void {

  }

  private getListingDetailsById(id: number): void {
    this.listingService.getListingDetailsById(id).subscribe({
      next: (resposne: ListingDetails) => {
        this.listing = resposne;
      }
    });
  }
}
