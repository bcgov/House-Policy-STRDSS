import { Component, OnInit } from '@angular/core';
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
export class TermsAndConditionsComponent implements OnInit {
  accepted = false;
  termsAndConditionsUrl = '';

  constructor(private userDataService: UserDataService) { }

  ngOnInit(): void {
    this.userDataService.getCurrentUser().subscribe({
      next: (user) => {
        switch (user.organizationType) {
          case 'LG':
            this.termsAndConditionsUrl = 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-toc-localgovernment';
            break;

          case 'Platform':
            this.termsAndConditionsUrl = 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-toc-platforms';
            break;

          default:
            break;
        }
      }
    })
  }

  accept(): void {
    this.userDataService.acceptTermsAndConditions().subscribe({
      next: () => {
        window.location.href = '/';
      }
    });
  }
}
