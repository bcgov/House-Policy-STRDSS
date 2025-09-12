import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { CheckboxChangeEvent, CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { validateEmailListString, validateUrl } from '../../../common/consts/validators.const';
import { HttpErrorResponse } from '@angular/common/http';
import { InputMaskModule } from 'primeng/inputmask';
import { TooltipModule } from 'primeng/tooltip';
import { ComplianceNotice } from '../../../common/models/compliance-notice';
import { MessageModule } from 'primeng/message';
import { Router } from '@angular/router';
import { InputNumberModule } from 'primeng/inputnumber';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { EditorModule, EditorTextChangeEvent } from 'primeng/editor';
import { ToastMessageOptions } from 'primeng/api';

@Component({
  selector: 'app-compliance-notice',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    TextareaModule,
    MessageModule,
    CheckboxModule,
    ChipsModule,
    DialogModule,
    TooltipModule,
    InputMaskModule,
    ButtonModule,
    EditorModule,
  ],
  templateUrl: './compliance-notice.component.html',
  styleUrl: './compliance-notice.component.scss'
})
export class ComplianceNoticeComponent implements OnInit {
  myForm!: FormGroup;

  platformOptions = new Array<DropdownOption>();

  isPreviewVisible = false;
  previewText = 'No preview'

  messages = new Array<ToastMessageOptions>();
  comment = '';

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

  constructor(
    private fb: FormBuilder,
    private delistingService: DelistingService,
    private router: Router,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
    private messageService: ErrorHandlingService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loaderService.loadingStart();

    this.delistingService.getPlatforms().subscribe({
      next: (platformOptions) => { this.platformOptions = platformOptions },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  onPreview(): void {
    this.messages = [];

    if (this.myForm.valid) {
      this.loaderService.loadingStart();
      this.delistingService.complianceNoticePreview(this.prepareFormModel(this.myForm))
        .subscribe(
          {
            next: preview => {
              this.previewText = preview.content;
              this.isPreviewVisible = true;
            },
            error: error => {
              this.showErrors(error);
            },
            complete: () => {
              this.loaderService.loadingEnd();
              this.cd.detectChanges();
            }
          }
        );
    } else {
      this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: 'Form is invalid' }];
      console.error('Form is invalid!');
    }
  }

  onSubmit(comment: string): void {
    this.messages = [];

    if (this.myForm.valid) {
      this.loaderService.loadingStart();
      const model: ComplianceNotice = this.prepareFormModel(this.myForm);
      model.comment = comment;

      this.delistingService.createComplianceNotice(model)
        .subscribe({
          next: (_) => {
            this.messageService.showSuccess('Your Notice of Non-Compliance was Successfully Submitted!');
          },
          error: (error) => {
            this.showErrors(error);
          },
          complete: () => {
            this.myForm.reset();
            this.initForm();
            this.onPreviewClose();
            this.comment = '';
            this.loaderService.loadingEnd();
            this.cd.detectChanges();
          }
        });
    }
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }

  onAlternativeDeliveryChanged(value: CheckboxChangeEvent): void {
    if (value.checked) {
      this.hostEmailControl.setValue('');
      this.hostEmailControl.removeValidators([Validators.required]);
    }
    else {
      this.hostEmailControl.addValidators([Validators.required]);
    }

    this.hostEmailControl.updateValueAndValidity();
    this.myForm.updateValueAndValidity();
  }

  cleanupPopupComment(commentTextArea: HTMLTextAreaElement): void {
    commentTextArea.value = '';
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
      platformId: [null, Validators.required],
      listingId: [''],
      listingUrl: ['', [Validators.required, validateUrl()]],
      hostEmail: ['', [Validators.required, Validators.email]],
      hostEmailSent: [false],
      reasonId: [1, Validators.required,],
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