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
import { UserDataService } from '../../../common/services/user-data.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

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
  currentUser: any;

  hideForm = false;
  showRequestedSuccessfullyMessage = false;
  showRequestedFailedMessage = false;
  alreadyClicked = false;

  public get organizationTypeControl(): AbstractControl {
    return this.myForm.controls['organizationType'];
  }
  public get organizationNameControl(): AbstractControl {
    return this.myForm.controls['organizationName'];
  }

  constructor(
    private fb: FormBuilder,
    private requestAccessService: RequestAccessService,
    private userDataService: UserDataService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.initData();
    this.initForm();
  }

  onRequest(): void {
    if (!this.alreadyClicked) {

      this.alreadyClicked = true;
      this.messages = [];
      this.showRequestedFailedMessage = false;
      this.showRequestedSuccessfullyMessage = false;
      const model: AccessRequest = this.myForm.getRawValue();
      this.loaderService.loadingStart();

      this.requestAccessService.createAccessRequest(model).subscribe({
        next: _ => {
          this.hideForm = true;
          this.showRequestedSuccessfullyMessage = true;
          this.messages = [];
        },
        error: (error: {
          error: {
            errors: {
              organizationType: string[],
              organizationName: string[],
              entity: string[]
            }
          }
        }) => {
          this.showRequestedFailedMessage = true;
          if (error.error.errors.entity) {
            this.messages = [{ severity: 'error', summary: 'Request cannot be sent!', detail: error.error.errors.entity[0] }];
          }
          if (error.error.errors.organizationType) {
            this.messages.push({ severity: 'error', summary: 'Request failed!', detail: error.error.errors.organizationType[0] });
          }
          if (error.error.errors.organizationName) {
            this.messages.push({ severity: 'error', summary: 'Request failed!', detail: error.error.errors.organizationName[0] });
          }
          if (!this.messages.length) {
            this.messages.push({ severity: 'error', summary: 'Request failed!', detail: 'Unhandled error.' });
          }
        },
        complete: () => {
          this.loaderService.loadingEnd();
        }
      });
    }
  }

  private initData(): void {
    this.requestAccessService.getOrganizationTypes().subscribe({
      next: (types => {
        this.roles = types;
      }),
    });
    this.userDataService.getCurrentUser().subscribe({
      next: user => {
        this.currentUser = user;
      }
    })
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationType: ['', Validators.required],
      organizationName: ['', Validators.required],
    });
  }
}
