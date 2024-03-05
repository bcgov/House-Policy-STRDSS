import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserToolsComponent } from '../user-tools/user-tools.component';
import { AuthenticationService } from '../../services/authentication.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, UserToolsComponent, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent implements OnInit {
  isUserLogged = false;

  constructor(private authService: AuthenticationService,) { }

  ngOnInit(): void {
    this.authService.userLoginSubject.subscribe((state) => {
      this.isUserLogged = state;
    })
  }
}
