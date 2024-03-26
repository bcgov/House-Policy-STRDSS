import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { validateEmailListString, validateUrl } from '../../../common/consts/validators.const';
import { ToastModule } from 'primeng/toast';
import { HttpErrorResponse } from '@angular/common/http';
import { Message } from 'primeng/api';
import { DelistingRequest } from '../../../common/models/delisting-request';
import { TooltipModule } from 'primeng/tooltip';
import { MessagesModule } from 'primeng/messages';
import { Router } from '@angular/router';

@Component({
  selector: 'app-delisting-request',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    InputTextareaModule,
    MessagesModule,
    CheckboxModule,
    CommonModule,
    ChipsModule,
    DialogModule,
    TooltipModule,
    ButtonModule,
    ToastModule,
  ],
  templateUrl: './delisting-request.component.html',
  styleUrl: './delisting-request.component.scss'
})
export class DelistingRequestComponent implements OnInit {
  myForm!: FormGroup;

  platformOptions = new Array<DropdownOption>();
  initiatorsOptions = new Array<DropdownOption>();

  isPreviewVisible = false;
  hideForm = false;
  previewText = 'No preview'

  messages = new Array<Message>();

  public get lgIdControl(): AbstractControl {
    return this.myForm.controls['lgId'];
  }
  public get platformIdControl(): AbstractControl {
    return this.myForm.controls['platformId'];
  }
  public get listingIdControl(): AbstractControl {
    return this.myForm.controls['listingId'];
  }
  public get listingUrlControl(): AbstractControl {
    return this.myForm.controls['listingUrl'];
  }
  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }

  constructor(private fb: FormBuilder, private delistingService: DelistingService, private router: Router) { }

  ngOnInit(): void {
    this.initForm();

    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
    this.delistingService.getLocalGovernments().subscribe((lgOptions) => this.initiatorsOptions = lgOptions);
  }

  onPreview(): void {
    if (this.myForm.valid) {
      const model: DelistingRequest = Object.assign({}, this.myForm.value);

      model.ccList = this.myForm.value['ccList'].prototype === Array
        ? this.myForm.value
        : (this.myForm.value['ccList'] as string).split(',').filter(x => !!x).map(x => x.trim())

      this.delistingService.delistingRequestPreview(model).subscribe(
        {
          next: preview => {
            this.previewText = preview.content;
            this.isPreviewVisible = true;
          },
          error: error => {
            this.showErrors(error);
          }
        }
      )
    } else {
      this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: 'Form is invalid' }];
      console.error('Form is invalid!');
    }
  }

  onSubmit(): void {
    if (this.myForm.valid) {
      const model: DelistingRequest = this.myForm.value;
      model.ccList = this.myForm.value['ccList'].prototype === Array
        ? this.myForm.value
        : (this.myForm.value['ccList'] as string).split(',').filter(x => !!x).map(x => x.trim())

      this.delistingService.createDelistingRequest(model)
        .subscribe({
          next: (_) => {
            this.showSuccessMessage();
          },
          error: (error) => {
            this.showErrors(error);
          },
          complete: () => {
            this.myForm.reset();
            this.initForm();
            this.onPreviewClose();
          }
        });
    }
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }

  onReturnHome(): void {
    this.router.navigateByUrl('/');
  }

  showSuccessMessage(): void {
    this.hideForm = true;
    this.messages = [{ severity: 'success', summary: '', detail: 'Your Notice of Takedown was Successfully Submitted!' }];
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      lgId: [0, Validators.required],
      platformId: [0, Validators.required],
      listingId: [null, [Validators.pattern(/\d+/)]],
      listingUrl: ['', [Validators.required, validateUrl()]],
      sendCopy: [true],
      ccList: ['', validateEmailListString()],
    });
  }

  private showErrors(error: HttpErrorResponse | any): void {
    let errorObject = typeof error.error === 'string' ? JSON.parse(error.error) : error.error;
    if (error.error['detail']) {
      this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: error.error['detail'] }];
    } else {
      const errorKeys = Object.keys(errorObject.errors)

      if (!errorKeys) {
        this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: 'Some properties are not valid' }];
      }
      else {
        errorKeys.forEach(key => {
          this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: errorObject.errors[key] }];
        });
      }
    }
  }
}