import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterOutlet, CardModule, ButtonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  constructor(private router: Router) {

  }

  toComplianceNotice(): void {
    this.router.navigateByUrl('/compliance-notice')
  }

  toDelistingRequest(): void {
    this.router.navigateByUrl('/delisting-request')
  }
}
