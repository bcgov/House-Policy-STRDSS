import { Component, OnInit } from '@angular/core';

import { LayoutComponent } from './common/layout/layout.component';
import { CommonModule } from '@angular/common';
import { KeycloakAngularModule } from 'keycloak-angular';
import { UserDataService } from './common/services/user-data.service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [CommonModule, KeycloakAngularModule, LayoutComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
    currentUserLoaded = false;

    constructor(private userService: UserDataService) { }

    ngOnInit(): void {
        this.userService.getCurrentUser().subscribe((user) => { this.currentUserLoaded = !!user })
    }
}
