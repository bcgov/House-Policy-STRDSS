import { Injectable } from '@angular/core';
import { ErrorBackEnd } from '../models/errors';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {
  public backendErrorSubject = new Subject<ErrorBackEnd>();
  public successSubject = new Subject<string>();
  public errorSubject = new Subject<string>();

  constructor() { }

  showErrorFromBackend(error: ErrorBackEnd): void {
    this.backendErrorSubject.next(error);
  }

  showError(message: string): void {
    this.errorSubject.next(message);
  }

  showSuccess(message: string): void {
    this.successSubject.next(message);
  }
}
