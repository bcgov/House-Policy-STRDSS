import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { UserDataService } from '../../../common/services/user-data.service';
import { CommonModule } from '@angular/common';
import { User } from '../../../common/models/user';
import { DashboardService } from '../../../common/services/dashboard.service';
import { DashboardCard, DashboardCardSections } from '../../../common/models/dashboard-card';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CardModule,
    ButtonModule,
    CommonModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  userType!: '' | 'BCGov' | 'Platform' | 'LG' | 'Admin';
  currentUser!: User;

  cardsToDisplay = new Array<DashboardCard>();
  cardSectionsToDisplay: DashboardCardSections = {
    main: [],
    admin: [],
    forms: [],
    info: [],
  }

  constructor(
    private router: Router,
    private userDataService: UserDataService,
    private dashboardService: DashboardService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    const cached = this.userDataService.currentUser;
    if (cached) {
      this.applyDashboardUser(cached);
      return;
    }

    this.loaderService.loadingStart();
    this.userDataService.getCurrentUser().subscribe({
      next: (value: User) => this.applyDashboardUser(value),
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  private applyDashboardUser(user: User): void {
    this.currentUser = user;
    this.cardSectionsToDisplay = this.dashboardService.getCardsPerUserType(user);
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
