import { Injectable } from '@angular/core';
import { ErrorBackEnd } from '../models/errors';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {
  public errorSubject = new Subject<ErrorBackEnd>();

  constructor() { }

  showError(error: ErrorBackEnd): void {
    this.errorSubject.next(error);
  }
}
