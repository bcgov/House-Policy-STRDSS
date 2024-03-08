import { Component, OnInit, SecurityContext } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { DialogModule } from 'primeng/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { ButtonModule } from 'primeng/button';

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

  constructor(private fb: FormBuilder, private delistingService: DelistingService, private sanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.myForm = this.fb.group({
      platformId: [null, Validators.required],
      listingUrl: ['', Validators.required],
      hostEmail: [''],
      reasonId: [null, Validators.required],
      sendCopy: [true],
      ccList: [[]],
      comment: [''],
    });

    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
    this.delistingService.getReasons().subscribe((reasonOptions) => this.reasonOptions = reasonOptions);
  }

  onPreview() {
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
}
