import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { InputMaskModule } from 'primeng/inputmask';
import { AuthenticationService } from '../../../services/authentication.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CardModule,
    InputTextModule,
    InputMaskModule,
    ButtonModule,
    PasswordModule,
    CommonModule,
    FormsModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  username!: string;
  password!: string;
  lastName!: string;
  addressStreet!: string;
  addressCity!: string;
  addressProvince!: string;
  addressPostalCode!: string;
  mobilePhone!: string;

  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.authService.signOut();
  }

  onSubmit(): void {
    this.authService
      .signUp({
        username: this.username,
        password: this.password,
        city: this.addressCity,
        postalCode: this.addressPostalCode,
        province: this.addressProvince,
        streetAddress: this.addressStreet,
        lastName: this.lastName,
        phoneNumber: this.mobilePhone,
      })
      .subscribe({
        next: (_value) => {
          this.router.navigate(['login']);
        },
        error: (e) => {
          if (e && e.status === 422 && e.error) {
            Object.keys(e.error.errors).forEach((errorKey: string) => {
              this.messageService.add({
                severity: 'error',
                summary: `${errorKey} validation error`,
                detail: (e.error.errors[errorKey] as Array<string>).toString(),
              });
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

  redirectToLogin(): void {
    this.router.navigate(['login']);
  }
}
