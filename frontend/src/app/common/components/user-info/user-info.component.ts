import { Component, OnInit } from '@angular/core';
import { ChipModule } from 'primeng/chip';
import { AuthenticationService } from '../../services/authentication.service';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MessagesModule } from 'primeng/messages';
import { NotificationMessage } from '../../models/notification-message.model';
import { NotificationMessagesApiService } from '../../services/api/notification-messages-api.service';
import { CommonModule } from '@angular/common';
import { Message } from 'primeng/api';
import { Router } from '@angular/router';
import { interval, startWith, switchMap } from 'rxjs';
@Component({
  selector: 'app-user-info',
  standalone: true,
  imports: [
    CommonModule,
    ChipModule,
    ButtonModule,
    BadgeModule,
    OverlayPanelModule,
    MessagesModule,
  ],
  templateUrl: './user-info.component.html',
  styleUrl: './user-info.component.scss'
})
export class UserInfoComponent implements OnInit {
  username!: string;
  messages = new Array<Message>();

  constructor(private authService: AuthenticationService, private notificationsService: NotificationMessagesApiService, private router: Router) { }

  ngOnInit(): void {
    this.username = this.authService.currentUser.username;
    interval(5000).pipe(
      startWith(0),
      switchMap(() => this.notificationsService.getNotifications())
    )
      .subscribe((messages: Array<NotificationMessage>) => {

        this.messages = messages.map(msg => {
          return { severity: 'warn', id: msg.id, summary: '', detail: msg.title }
        });
      })
  }

  signOut(): void {
    this.router.navigate(['login']);
    this.authService.signOut();
  }
}
