import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ListingTableRow } from '../../../../common/models/listing-table-row';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ListingDataService } from '../../../../common/services/listing-data.service';
import { ListingDetails } from '../../../../common/models/listing-details';

@Component({
  selector: 'app-listing-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    PanelModule,
  ],
  templateUrl: './listing-details.component.html',
  styleUrl: './listing-details.component.scss'
})
export class ListingDetailsComponent implements OnInit {
  id!: number;
  listing!: ListingDetails;

  constructor(private route: ActivatedRoute, private listingService: ListingDataService) {
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];

    this.getListingDetailsById(this.id);
  }

  showLegend(): void {
  }

  private getListingDetailsById(id: number): void {
    this.listingService.getListingDetailsById(id).subscribe({
      next: (resposne: ListingDetails) => {
        this.listing = resposne;
      }
    });
  }
}
