import { Component, OnInit } from '@angular/core';
import { Message } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { MessagesModule } from 'primeng/messages';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { RequestAccessService } from '../../../common/services/request-access.service';
import { AccessRequest } from '../../../common/models/access-request';

@Component({
  selector: 'app-access-request',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    CardModule,
    ButtonModule,
    DropdownModule,
    MessagesModule,
    InputTextModule,
  ],
  templateUrl: './access-request.component.html',
  styleUrl: './access-request.component.scss'
})
export class AccessRequestComponent implements OnInit {
  myForm!: FormGroup;

  messages = new Array<Message>();
  roles = new Array<DropdownOption>();

  hideForm = false;

  public get organizationTypeControl(): AbstractControl {
    return this.myForm.controls['organizationType'];
  }
  public get organizationNameControl(): AbstractControl {
    return this.myForm.controls['organizationName'];
  }

  constructor(
    private fb: FormBuilder,
    private requestAccessService: RequestAccessService,
  ) { }

  ngOnInit(): void {
    this.initForm();
  }

  onRequest(): void {
    const model: AccessRequest = this.myForm.getRawValue()
    this.requestAccessService.createAccessRequest(model).subscribe({
      next: res => {
        console.log(res);
        this.hideForm = true;
      },
      error: (error: {
        errors: {
          organizationType: string[],
          organizationName: string[],
        }
      }) => {

      }
    })
  }

  private initForm(): void {
    this.requestAccessService.getOrganizationTypes().subscribe({
      next: (types => {
        this.roles = types;
      }),
    });

    this.myForm = this.fb.group({
      organizationType: [0, Validators.required],
      organizationName: ['', Validators.required],
    });
  }
}
