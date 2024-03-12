import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, ButtonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  showHeaderMenu = true;

  constructor(private router: Router) {
  }

  onHomeClicked(): void {
    this.router.navigateByUrl('/');
  }
}
