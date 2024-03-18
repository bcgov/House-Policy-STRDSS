import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { MessageService } from 'primeng/api';
import { DelistingRequest } from '../../../common/models/delisting-request';
import { TooltipModule } from 'primeng/tooltip';
import { MessagesModule } from 'primeng/messages';

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
  previewText = 'No preview'

  constructor(private fb: FormBuilder, private delistingService: DelistingService, private messageService: MessageService) { }

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
      this.messageService.add({ severity: 'error', summary: 'Validation error', detail: "Form is invalid" });
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
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Message has been sent successfully' });
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

  private initForm(): void {
    this.myForm = this.fb.group({
      lgId: [0, Validators.required],
      platformId: [0, Validators.required],
      listingId: [null],
      listingUrl: ['', [Validators.required, validateUrl()]],
      sendCopy: [true],
      ccList: ['', validateEmailListString()],
    });
  }

  showErrors(error: HttpErrorResponse | any): void {
    let errorObject = typeof error.error === 'string' ? JSON.parse(error.error) : error.error;
    if (error.error['detail']) {
      this.messageService.add({ severity: 'error', summary: 'Validation error', detail: error.error['detail'], life: 10000 });
    } else {
      const errorKeys = Object.keys(errorObject.errors)

      if (!errorKeys) {
        this.messageService.add({ severity: 'error', summary: 'Validation error', detail: 'Some properties are not valid' });
      }
      else {
        errorKeys.forEach(key => {
          this.messageService.add({ severity: 'error', summary: 'Validation error', detail: errorObject.errors[key] });
        });
      }
    }
  }
}
