import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { NotificationMessage } from '../../models/notification-message.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationMessagesApiService {
  private readonly apiUrl = `api`;

  constructor(private httpClient: HttpClient) {
  }

  getNotifications(): Observable<Array<NotificationMessage>> {
    return this.httpClient.get<Array<NotificationMessage>>(`${this.apiUrl}/notifications`)
  }

}
