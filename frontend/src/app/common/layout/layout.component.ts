import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { UserDataService } from '../services/user-data.service';
import { AuthService } from '../services/auth.service';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CommonModule } from '@angular/common';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ErrorHandlingService } from '../services/error-handling.service';
import { ErrorBackEnd } from '../models/errors';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, ButtonModule, MenubarModule, OverlayPanelModule, CommonModule, ToastModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  showHeaderMenu = true;
  items: MenuItem[] | undefined;

  constructor(public userDataService: UserDataService, private authService: AuthService, private messageService: MessageService, private errorHandlingService: ErrorHandlingService) { }

  ngOnInit() {
    this.errorHandlingService.errorSubject.subscribe({
      next: (error) => {
        this.showError(error);
      }
    });

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
  private showError(error: ErrorBackEnd) {
    if (!!error.errors) {
      Object.keys(error.errors).forEach(oneError => {
        this.messageService.add({ severity: 'error', summary: error.title, detail: error.errors[oneError].join(' ') });
      });
    } else {
      if (!!error.title && !!error.detail) {
        this.messageService.add({ severity: 'error', summary: error.title, detail: error.detail });
      } else {
        this.messageService.add({ severity: 'error', summary: error.statusText, detail: "Check console for details" });
      }
    }
    if (!!error.traceId) {
      console.info('TraceID:', error.traceId);
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
