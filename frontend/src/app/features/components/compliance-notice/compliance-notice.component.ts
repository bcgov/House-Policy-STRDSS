import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxChangeEvent, CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { validateEmailListString, validateUrl } from '../../../common/consts/validators.const';
import { Message } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';
import { InputMaskModule } from 'primeng/inputmask';
import { TooltipModule } from 'primeng/tooltip';
import { ComplianceNotice } from '../../../common/models/compliance-notice';
import { MessagesModule } from 'primeng/messages';
import { Router } from '@angular/router';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-compliance-notice',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    InputTextareaModule,
    MessagesModule,
    CheckboxModule,
    ChipsModule,
    DialogModule,
    TooltipModule,
    InputMaskModule,
    ButtonModule,
  ],
  templateUrl: './compliance-notice.component.html',
  styleUrl: './compliance-notice.component.scss'
})
export class ComplianceNoticeComponent implements OnInit {
  myForm!: FormGroup;

  platformOptions = new Array<DropdownOption>();
  reasonOptions = new Array<DropdownOption>();

  isPreviewVisible = false;
  hideForm = false;
  previewText = 'No preview'

  messages = new Array<Message>();

  public get platformIdControl(): AbstractControl {
    return this.myForm.controls['platformId'];
  }
  public get listingUrlControl(): AbstractControl {
    return this.myForm.controls['listingUrl'];
  }
  public get hostEmailControl(): AbstractControl {
    return this.myForm.controls['hostEmail'];
  }
  public get reasonIdControl(): AbstractControl {
    return this.myForm.controls['reasonId'];
  }
  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }
  public get lgContactEmailControl(): AbstractControl {
    return this.myForm.controls['LgContactEmail'];
  }
  public get lgContactPhoneControl(): AbstractControl {
    return this.myForm.controls['LgContactPhone'];
  }
  public get strBylawUrlControl(): AbstractControl {
    return this.myForm.controls['StrBylawUrl'];
  }

  constructor(private fb: FormBuilder, private delistingService: DelistingService, private router: Router) { }

  ngOnInit(): void {
    this.initForm();

    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
    this.delistingService.getReasons().subscribe((reasonOptions) => this.reasonOptions = reasonOptions);
  }

  onPreview(): void {
    this.messages = [];

    if (this.myForm.valid) {
      this.delistingService.complianceNoticePreview(this.prepareFormModel(this.myForm))
        .subscribe(
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

  onSubmit(comment: string, textAreaElement: HTMLTextAreaElement): void {
    this.messages = [];

    if (this.myForm.valid) {
      const model: ComplianceNotice = this.prepareFormModel(this.myForm);
      model.comment = comment;

      this.delistingService.createComplianceNotice(model)
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
            this.cleanupPopupComment(textAreaElement);
          }
        });
    }
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }

  onAlternativeDeliveryChanged(value: CheckboxChangeEvent): void {
    if (value.checked)
      this.hostEmailControl.removeValidators([Validators.required]);
    else
      this.hostEmailControl.addValidators([Validators.required]);

    this.hostEmailControl.updateValueAndValidity();
    this.myForm.updateValueAndValidity();
  }

  cleanupPopupComment(commentTextArea: HTMLTextAreaElement): void {
    commentTextArea.value = '';
  }

  onReturnHome(): void {
    this.router.navigateByUrl('/');
  }

  showSuccessMessage(): void {
    this.hideForm = true;
    this.messages = [{ severity: 'success', summary: '', detail: 'Your Notice of Takedown was Successfully Submitted!' }];
  }

  private prepareFormModel(form: FormGroup): ComplianceNotice {
    const model: ComplianceNotice = Object.assign({}, form.value);

    model.ccList = form.value['ccList'].prototype === Array
      ? form.value
      : (form.value['ccList'] as string).split(',').filter(x => !!x).map(x => x.trim())

    return model;
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      platformId: [0, Validators.required],
      listingId: [''],
      listingUrl: ['', [Validators.required, validateUrl()]],
      hostEmail: ['', [Validators.required, Validators.email]],
      hostEmailSent: [false],
      reasonId: [0, Validators.required,],
      sendCopy: [true],
      ccList: ['', validateEmailListString()],
      LgContactEmail: ['', [Validators.required, Validators.email]],
      LgContactPhone: [''],
      StrBylawUrl: ['', validateUrl()],
      comment: [''],
    });
  }

  private showErrors(error: HttpErrorResponse | any): void {
    let errorObject = typeof error.error === 'string' ? JSON.parse(error.error) : error.error;
    if (error.error['detail']) {
      this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: error.error['detail'] }];
    } else {
      const errorKeys = Object.keys(errorObject.errors)

      if (!errorKeys) {
        this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: 'Some properties are not valid' }];
      }
      else {
        errorKeys.forEach(key => {
          this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: errorObject.errors[key] }];
        });
      }
    }
  }
}