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
import { validateEmailChips, validateUrl } from '../../../common/consts/validators.const';
import { ToastModule } from 'primeng/toast';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-delisting-request',
  standalone: true,
  imports: [ReactiveFormsModule, DropdownModule, InputTextModule, InputTextareaModule,
    CheckboxModule, CommonModule, ChipsModule, DialogModule, ButtonModule, ToastModule],
  templateUrl: './delisting-request.component.html',
  styleUrl: './delisting-request.component.scss'
})
export class DelistingRequestComponent implements OnInit {
  myForm!: FormGroup;

  platformOptions = new Array<DropdownOption>();
  reasonOptions = new Array<DropdownOption>();

  isPreviewVisible = false;
  previewText = 'No preview'

  constructor(private fb: FormBuilder, private delistingService: DelistingService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.initForm();

    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
    this.delistingService.getReasons().subscribe((reasonOptions) => this.reasonOptions = reasonOptions);
  }

  onPreview(): void {
    if (this.myForm.valid) {
      this.delistingService.delistingRequestPreview(this.myForm.value).subscribe(
        {
          next: preview => {
            this.previewText = preview;
            this.isPreviewVisible = true;
          },
          error: error => {
            this.showErrors(error);
          }
        }
      )
    } else {
      console.error('Form is invalid!');
    }
  }

  onSubmit(comment: string): void {
    if (this.myForm.valid) {
      const formValue = this.myForm.value;
      formValue.comment = comment;

      this.delistingService.createDelistingRequest(formValue).subscribe({
        next: (_) => {
          this.myForm.reset();
          this.initForm();
          this.onPreviewClose();
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Message has been sent successfully' });
        },
        error: (error) => {
          this.myForm.reset();
          this.initForm();
          this.onPreviewClose();
          this.showErrors(error);
        }
      })
    }
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      platformId: [0, Validators.required],
      listingUrl: ['', [Validators.required, validateUrl()]],
      sendCopy: [true],
      ccList: [[], validateEmailChips()],
      comment: [''],
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
