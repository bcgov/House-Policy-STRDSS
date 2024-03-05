import { Component, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../../services/authentication.service';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CardModule,
    InputTextModule,
    ButtonModule,
    PasswordModule,
    CommonModule,
    FormsModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  username!: string;
  password!: string;

  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.authService.signOut();
  }

  onSubmit(): void {
    localStorage.removeItem('access_token');
    this.authService.signIn(this.username, this.password).subscribe({
      next: (_value) => {
        this.router.navigate(['dashboard']);
      },
      error: (e) => {
        if (e && e.status === 422 && e.error.errors['Password']) {
          this.messageService.add({
            severity: 'warn',
            summary: 'Wrong credentials',
            detail: e.error.errors['Password'],
          });
        } else {
          this.messageService.add({
            severity: 'error',
            summary: 'Unkonw error',
            detail: e.status,
          });
        }
      },
    });
  }

  redirectToSignUp(): void {
    this.router.navigate(['register']);
  }
}
