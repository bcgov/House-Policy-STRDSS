import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { UserDataService } from '../../../common/services/user-data.service';
import { CommonModule } from '@angular/common';
import { User } from '../../../common/models/user';
import { DashboardService } from '../../../common/services/dashboard.service';
import { DashboardCard } from '../../../common/models/dashboard-card';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterOutlet,
    CardModule,
    ButtonModule,
    CommonModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  userType = '' || 'BCGov' || 'Platform' || 'LG' || 'Admin';
  currentUser!: User;

  cardsToDisplay = new Array<DashboardCard>();

  constructor(
    private router: Router,
    private userDataService: UserDataService,
    private dashboardService: DashboardService
  ) { }

  ngOnInit(): void {
    this.userDataService.getCurrentUser().subscribe({
      next: (value: User) => {
        this.currentUser = value;

        this.cardsToDisplay = this.filterCardsByUserClaims(this.dashboardService.getCards());
      },
    })
  }

  filterCardsByUserClaims(cardsFromService: Array<DashboardCard>): Array<DashboardCard> {
    const cards = new Array<DashboardCard>();

    cardsFromService.forEach((card) => {
      if (card.isItOrgTypeBased) {
        if (this.currentUser.organizationType === card.orgType) {
          cards.push(card);
        }
      } else {
        if (this.currentUser.permissions.includes(card.accessPermission)) {
          cards.push(card);
        }
      }
    });

    return cards;
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
