import { Component } from '@angular/core';
import { ApplicationsComponent } from '../applications/applications.component';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    ApplicationsComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  ngOnInit(): void {
  }
}
