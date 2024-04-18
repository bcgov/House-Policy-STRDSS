import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { UserDataService } from '../services/user-data.service';
import { AuthService } from '../services/auth.service';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, ButtonModule, MenubarModule, OverlayPanelModule, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  showHeaderMenu = true;
  items: MenuItem[] | undefined;

  constructor(public userDataService: UserDataService, private authService: AuthService) { }

  ngOnInit() {
    this.items = [
      {
        label: 'Home',
        routerLink: '/',
      },
      {
        label: 'Listings',
        routerLink: '/',
        disabled: true,
      },
      {
        label: 'Alerts',
        routerLink: '/',
        disabled: true,
      },
      {
        label: 'Upload',
        items: [
          {
            label: 'Upload Listing Data',
            routerLink: '',
            disabled: true,
          },
          {
            label: 'Platform Upload History',
            routerLink: '',
            disabled: true,
          },
        ]
      },
      {
        label: 'Admin tools',
        items: [
          {
            label: 'User Management',
            routerLink: '/user-management',
          },
        ]
      },
    ];
  }

  logout(): void {
    this.authService.logout();
  }
}
