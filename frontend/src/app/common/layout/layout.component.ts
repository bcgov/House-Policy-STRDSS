import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, ButtonModule, MenubarModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  showHeaderMenu = true;
  items: MenuItem[] | undefined;

  constructor(private router: Router) {
  }

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
        visible: false,
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
}
