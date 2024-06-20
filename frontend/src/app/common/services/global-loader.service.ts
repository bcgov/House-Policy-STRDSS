import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GlobalLoaderService {
  loadingNotification = new Subject<{ isLoading: boolean, title?: string }>()

  constructor() { }

  loadingStart(title: string = 'Loading'): void {
    this.loadingNotification.next({ isLoading: true, title });
  }

  loadingEnd(): void {
    this.loadingNotification.next({ isLoading: false, title: '' });
  }
}
