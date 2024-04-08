import { Component } from '@angular/core';
import { UserDataService } from '../../services/user-data.service';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
  selector: 'app-terms-and-conditions',
  standalone: true,
  imports: [ButtonModule, CardModule, FormsModule, CommonModule, CheckboxModule],
  templateUrl: './terms-and-conditions.component.html',
  styleUrl: './terms-and-conditions.component.scss'
})
export class TermsAndConditionsComponent {
  accepted = false;

  constructor(private userDataService: UserDataService) { }

  accept(): void {
    this.userDataService.acceptTermsAndConditions().subscribe({
      next: () => {
        window.location.href = '/';
      }
    });
  }
}
