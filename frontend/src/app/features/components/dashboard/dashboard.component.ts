import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { UserDataService } from '../../../common/services/user-data.service';
import { CommonModule } from '@angular/common';
import { User } from '../../../common/models/user';
import { DashboardService } from '../../../common/services/dashboard.service';
import { DashboardCard, DashboardCardSections } from '../../../common/models/dashboard-card';


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

  /** True until profile-driven cards are ready (avoids empty-then-full CLS and holds aria-busy). */
  isDashboardCardsPending = true;

  readonly skeletonCardPlaceholders: number[] = [0, 1, 2];

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
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    const cached = this.userDataService.currentUser;
    if (cached) {
      this.applyDashboardUser(cached);
      return;
    }

    this.userDataService.getCurrentUser().subscribe({
      next: (value: User) => this.applyDashboardUser(value),
      error: () => {
        this.isDashboardCardsPending = false;
        this.cd.detectChanges();
      },
    });
  }

  private applyDashboardUser(user: User): void {
    this.isDashboardCardsPending = false;
    this.currentUser = user;
    this.cardSectionsToDisplay = this.dashboardService.getCardsPerUserType(user);
    this.cd.detectChanges();
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
