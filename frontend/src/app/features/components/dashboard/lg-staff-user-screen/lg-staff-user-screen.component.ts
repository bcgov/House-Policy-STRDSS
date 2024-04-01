import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-lg-staff-user-screen',
  standalone: true,
  imports: [CardModule, ButtonModule, RouterOutlet],
  templateUrl: './lg-staff-user-screen.component.html',
  styleUrl: './lg-staff-user-screen.component.scss'
})
export class LgStaffUserScreenComponent {
  constructor(private router: Router) {

  }

  toComplianceNotice(): void {
    this.router.navigateByUrl('/compliance-notice')
  }

  toDelistingRequest(): void {
    this.router.navigateByUrl('/delisting-request')
  }
}
