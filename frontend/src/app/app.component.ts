import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';

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
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
    currentUserLoaded = false;
    isLoading = true;
    loaderTitle? = 'Loading'
    isCurrentUserLoadedWithError = false;
    errorText = '';

    constructor(
        private userService: UserDataService,
        private loaderService: GlobalLoaderService,
        private cd: ChangeDetectorRef,
    ) { }

    ngOnInit(): void {
        this.loaderService.loadingNotification.subscribe({
            next: ({ isLoading, title }) => {
                this.isLoading = isLoading;
                this.loaderTitle = title;
                this.cd.detectChanges();
            },
        })

        this.userService.getCurrentUser().subscribe({
            next: (user) => {
                this.currentUserLoaded = !!user;
            },
            error: (err) => {
                if (err.error) {
                    this.errorText = `${err.error.detail}: ${err.error.instance}`
                }

                this.isCurrentUserLoadedWithError = true;
                this.loaderService.loadingEnd();
                this.cd.detectChanges();
            },
            complete: () => {
                this.loaderService.loadingEnd();
            },
        })
    }
}
