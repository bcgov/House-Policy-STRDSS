import { Component, OnInit } from '@angular/core';

import { LayoutComponent } from './common/layout/layout.component';
import { CommonModule } from '@angular/common';
import { KeycloakAngularModule } from 'keycloak-angular';
import { UserDataService } from './common/services/user-data.service';
import { GlobalLoaderService } from './common/services/global-loader.service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [CommonModule, KeycloakAngularModule, LayoutComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
    currentUserLoaded = false;
    isLoading = true;
    loaderTitle? = 'Loading'

    constructor(private userService: UserDataService, private loaderService: GlobalLoaderService) { }

    ngOnInit(): void {
        this.loaderService.loadingNotification.subscribe({
            next: ({ isLoading, title }) => {
                this.isLoading = isLoading;
                this.loaderTitle = title;
            },
        })

        this.userService.getCurrentUser().subscribe({
            next: (user) => {
                this.currentUserLoaded = !!user;
            },
            complete: () => {
                this.loaderService.loadingEnd();
            },
        })
    }
}
