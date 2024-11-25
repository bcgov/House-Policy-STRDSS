import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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
import { ToastModule } from 'primeng/toast';
import { HttpErrorResponse } from '@angular/common/http';
import { Message } from 'primeng/api';
import { DelistingRequest } from '../../../common/models/delisting-request';
import { TooltipModule } from 'primeng/tooltip';
import { MessagesModule } from 'primeng/messages';
import { Router } from '@angular/router';
import { InputNumberModule } from 'primeng/inputnumber';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { forkJoin } from 'rxjs';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { EditorModule, EditorTextChangeEvent } from 'primeng/editor';

@Component({
  selector: 'app-delisting-request',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    InputTextareaModule,
    MessagesModule,
    CheckboxModule,
    CommonModule,
    ChipsModule,
    DialogModule,
    TooltipModule,
    ButtonModule,
    ToastModule,
    EditorModule,
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

  messages = new Array<Message>();

  public get lgIdControl(): AbstractControl {
    return this.myForm.controls['lgId'];
  }
  public get platformIdControl(): AbstractControl {
    return this.myForm.controls['platformId'];
  }
  public get listingUrlControl(): AbstractControl {
    return this.myForm.controls['listingUrl'];
  }
  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }
  public get isWithStandardDetailControl(): AbstractControl {
    return this.myForm.controls['isWithStandardDetail'];
  }
  public get customDetailTxtControl(): AbstractControl {
    return this.myForm.controls['customDetailTxt'];
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
    const getPlatforms = this.delistingService.getPlatforms();
    const getLgOptions = this.delistingService.getLocalGovernments();

    forkJoin([getPlatforms, getLgOptions]).subscribe({
      next: ([platformOptions, lgOptions]) => {
        this.platformOptions = platformOptions;
        this.initiatorsOptions = lgOptions;
      },
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
      this.delistingService.delistingRequestPreview(this.prepareFormModel(this.myForm))
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
            }
          }
        );
    } else {
      this.messages = [{ severity: 'error', summary: 'Validation error', closable: true, detail: 'Form is invalid' }];
      console.error('Form is invalid!');
    }
  }

  onSubmit(): void {
    this.messages = [];

    if (this.myForm.valid) {
      this.loaderService.loadingStart();
      this.delistingService.createDelistingRequest(this.prepareFormModel(this.myForm))
        .subscribe({
          next: (_) => {
            this.messageService.showSuccess('Your Takedown Request was Successfully Submitted!');
            this.myForm.reset();
          },
          error: (error) => {
            this.showErrors(error);
          },
          complete: () => {
            this.myForm.reset();
            this.initForm();
            this.onPreviewClose();
            this.loaderService.loadingEnd();
          }
        });
    }
  }

  onEditorChanged(_: EditorTextChangeEvent): void {
    this.customDetailTxtControl.updateValueAndValidity();
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }

  onWithStandardDetailChanged(value: CheckboxChangeEvent): void {
    if (value.checked)
      this.customDetailTxtControl.removeValidators([Validators.required]);
    else
      this.customDetailTxtControl.addValidators([Validators.required]);

    this.customDetailTxtControl.updateValueAndValidity();
    this.myForm.updateValueAndValidity();
  }

  private prepareFormModel(form: FormGroup): DelistingRequest {
    const model: DelistingRequest = Object.assign({}, form.value);
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
      sendCopy: [true],
      ccList: ['', validateEmailListString()],
      isWithStandardDetail: [true],
      customDetailTxt: [''],
    });
  }

  private showErrors(error: HttpErrorResponse | any): void {
    let errorObject = typeof error.error === 'string' ? JSON.parse(error.error) : error.error;

    if (error.error['detail']) {
      this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: `${error.error['detail']}` }];
    } else {
      const errorKeys = Object.keys(errorObject.errors)

      if (!errorKeys) {
        this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: 'Some properties are not valid' }];
      }
      else {
        errorKeys.forEach(key => {
          this.messages = [{ severity: 'error', summary: 'Server error', closable: true, detail: `${errorObject.errors[key]}` }];
        });
      }
    }
  }
}