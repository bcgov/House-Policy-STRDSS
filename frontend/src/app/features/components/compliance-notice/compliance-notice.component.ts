import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { CommonModule } from '@angular/common';
import { ChipsModule } from 'primeng/chips';

@Component({
  selector: 'app-compliance-notice',
  standalone: true,
  imports: [ReactiveFormsModule, DropdownModule, InputTextModule, InputTextareaModule, CheckboxModule, CommonModule, ChipsModule],
  templateUrl: './compliance-notice.component.html',
  styleUrl: './compliance-notice.component.scss'
})
export class ComplianceNoticeComponent implements OnInit {

  myForm!: FormGroup;

  platformOptions = [
    { value: 1, label: 'Expedia' },
    { value: 2, label: 'Airbnb' },
    { value: 3, label: 'Booking.com' },
    { value: 4, label: 'VRBO' },
    { value: 5, label: 'TripAdvisor' },
    { value: 6, label: 'HomeToGo' },
    { value: 7, label: 'Tripping' },
    { value: 8, label: 'Homestay.com' },
    { value: 9, label: 'Atraveo' },
    { value: 10, label: 'OneFineStay' },
    { value: 11, label: 'Interhome' },
    { value: 12, label: '9flats' },
  ];

  reasonOptions = [
    { value: 1, label: 'No business license provided' },
    { value: 2, label: 'Invalid business licence number' },
    { value: 3, label: 'Expired business licence' },
    { value: 4, label: 'Suspended business license' },
  ];

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.myForm = this.fb.group({
      platformId: ['', Validators.required],
      listingUrl: ['', Validators.required],
      hostEmailAddress: [''],
      reasonId: ['', Validators.required],
      sendCopy: [true],
      additionalCcs: [''],
      comments: [''],
    });
  }

  onSubmit() {
    if (this.myForm.valid) {
      console.log('Form submitted:', this.myForm.value);
      // Send form data to your backend API or perform other actions
    } else {
      console.error('Form is invalid!');
    }
  }
}
