import { ChangeDetectorRef, Component } from '@angular/core';
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
import { TopMenuService } from '../services/top-menu.service';
import { TooltipModule } from 'primeng/tooltip';
import { listing_read } from '../consts/permissions.const';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, ButtonModule, MenubarModule, TooltipModule, OverlayPanelModule, CommonModule, ToastModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  showHeaderMenu = true;
  items: MenuItem[] | undefined;

  constructor(
    public userDataService: UserDataService,
    private topMenuService: TopMenuService,
    private authService: AuthService,
    private messageService: MessageService,
    private errorHandlingService: ErrorHandlingService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit() {
    this.errorHandlingService.backendErrorSubject.subscribe({
      next: (error) => {
        this.showBackendError(error);
      },
      complete: () => {
        this.cd.detectChanges();
      }
    });

    this.errorHandlingService.successSubject.subscribe({
      next: (message: string) => {
        this.showSuccess(message);
      },
      complete: () => {
        this.cd.detectChanges();
      }
    });

    this.errorHandlingService.errorSubject.subscribe({
      next: (message: string) => {
        this.showError(message);
      },
      complete: () => {
        this.cd.detectChanges();
      }
    });

    this.items = [
      {
        label: 'Forms',
        items: []
      },
      {
        label: 'Upload',
        items: []
      },
      {
        label: 'Admin Tools',
        items: []
      },
      {
        label: 'Policy Guidance',
        items: []
      },
    ];
    this.initMenu();
  }

  private initMenu(): void {

    const usersMenuItems = this.topMenuService.getMenuItemsPerUserType(this.userDataService.currentUser);
    this.items?.forEach((folder) => {
      folder.items = usersMenuItems
        .filter((mItem) => mItem.folderName === folder.label)
        .map((mItem) => {
          return {
            id: mItem.buttonId,
            visible: mItem.hidden,
            disabled: mItem.disabled,
            url: mItem.route.toLowerCase().startsWith('http') ? mItem.route : '',
            target: mItem.route.toLowerCase().startsWith('http') ? '_blank' : '_self',
            routerLink: mItem.route.toLowerCase().startsWith('/') ? mItem.route : '',
            label: mItem.title,
          } as MenuItem
        });
    })

    this.items = this.items?.filter((item) => {
      return !!item.items?.length;
    });

    if (this.userDataService.currentUser.permissions.includes(listing_read)) {
      this.items?.unshift({
        label: 'Listings',
        routerLink: '/listings',
      });
    }

    this.items?.unshift({
      label: 'Home',
      routerLink: '/',
    });
  }

  private showBackendError(error: ErrorBackEnd): void {
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

  showSuccess(message: string): void {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: message });
  }

  showError(message: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }

  logout(): void {
    this.authService.logout();
  }
}
