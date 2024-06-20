import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { UserDataService } from '../../../common/services/user-data.service';
import { CommonModule } from '@angular/common';
import { User } from '../../../common/models/user';
import { DashboardService } from '../../../common/services/dashboard.service';
import { DashboardCard } from '../../../common/models/dashboard-card';
import { ListingUploadHistoryTableComponent } from '../../../common/listing-upload-history-table/listing-upload-history-table.component';
import { listing_file_upload } from '../../../common/consts/permissions.const';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterOutlet,
    CardModule,
    ButtonModule,
    CommonModule,
    ListingUploadHistoryTableComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  userType = '' || 'BCGov' || 'Platform' || 'LG' || 'Admin';
  currentUser!: User;
  showListingHistory = false;

  cardsToDisplay = new Array<DashboardCard>();

  constructor(
    private router: Router,
    private userDataService: UserDataService,
    private dashboardService: DashboardService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.userDataService.getCurrentUser().subscribe({
      next: (value: User) => {
        this.currentUser = value;
        this.cardsToDisplay = this.dashboardService.getCardsPerUserType(this.currentUser);
        this.showListingHistory = this.currentUser.permissions.includes(listing_file_upload);
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    })
  }

  navigateTo(route: string): void {
    if (route) {
      if (route.toLowerCase().startsWith('http')) {
        window.open(route);
      } else {
        this.router.navigateByUrl(route);
      }
    }
  }
}
