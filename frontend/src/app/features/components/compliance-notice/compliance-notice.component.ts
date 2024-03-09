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

@Component({
  selector: 'app-compliance-notice',
  standalone: true,
  imports: [ReactiveFormsModule, DropdownModule, InputTextModule, InputTextareaModule,
    CheckboxModule, CommonModule, ChipsModule, DialogModule, ButtonModule],
  templateUrl: './compliance-notice.component.html',
  styleUrl: './compliance-notice.component.scss'
})
export class ComplianceNoticeComponent implements OnInit {

  myForm!: FormGroup;

  platformOptions = new Array<DropdownOption>();
  reasonOptions = new Array<DropdownOption>();

  isPreviewVisible = false;
  previewText = 'No preview'

  constructor(private fb: FormBuilder, private delistingService: DelistingService) { }

  ngOnInit(): void {
    this.initForm();

    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
    this.delistingService.getReasons().subscribe((reasonOptions) => this.reasonOptions = reasonOptions);
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      platformId: [null, Validators.required],
      listingUrl: ['', [Validators.required, validateUrl()]],
      hostEmail: ['', Validators.email],
      reasonId: [null, Validators.required,],
      sendCopy: [true],
      ccList: [[], validateEmailChips()],
      comment: [''],
    });
  }

  onPreview(): void {
    if (this.myForm.valid) {
      this.delistingService.complianceNoticePreview(this.myForm.value).subscribe(
        {
          next: preview => {
            this.previewText = preview;
            this.isPreviewVisible = true;
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

      this.delistingService.createComplianceNotice(formValue).subscribe((_) => {
        this.myForm.reset();
        this.initForm();
      })
    }
  }

  onPreviewClose(): void {
    this.isPreviewVisible = false;
  }
}
