import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { OrganizationService } from '../../../../common/services/organization.service';
import { DropdownOption } from '../../../../common/models/dropdown-option';
import { RadioButtonModule } from 'primeng/radiobutton';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-new-platform',
  standalone: true,
  imports: [
    CommonModule,
    InputTextModule,
    DropdownModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    RadioButtonModule,
  ],
  templateUrl: './add-new-platform.component.html',
  styleUrl: './add-new-platform.component.scss'
})
export class AddNewPlatformComponent implements OnInit {
  myForm!: FormGroup;
  platformTypes = new Array<DropdownOption>();

  constructor(
    private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
    private router: Router,
    private orgService: OrganizationService,
  ) { }

  ngOnInit(): void {
    this.platformTypes = [
      {
        label: 'Minor (Less than 1,000 Listings)',
        value: 'Minor'
      },
      {
        label: 'Major (More than 1,000 Listings)',
        value: 'Major'
      },
    ];

    this.initForm();
  }

  onSave(): void {
    const platform = this.myForm.getRawValue();
    this.loaderService.loadingStart();
    this.orgService.addPlatform(platform).subscribe({
      next: (_) => {
        this.messageHandlerService.showSuccess('New platform has been added successfully')
        this.onCancel();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  onCancel(): void {
    this.myForm.reset();
    this.platformStatusControl.setValue(true);
    this.router.navigateByUrl(`/platform-management`);
  }

  public get organizationNmControl(): AbstractControl {
    return this.myForm.controls['organizationNm'];
  }
  public get organizationCdControl(): AbstractControl {
    return this.myForm.controls['organizationCd'];
  }
  public get primaryNoticeOfTakedownContactEmailControl(): AbstractControl {
    return this.myForm.controls['primaryNoticeOfTakedownContactEmail'];
  }
  public get primaryTakedownRequestContactEmailControl(): AbstractControl {
    return this.myForm.controls['primaryTakedownRequestContactEmail'];
  }
  public get secondaryNoticeOfTakedownContactEmailControl(): AbstractControl {
    return this.myForm.controls['secondaryNoticeOfTakedownContactEmail'];
  }
  public get secondaryTakedownRequestContactEmailControl(): AbstractControl {
    return this.myForm.controls['secondaryTakedownRequestContactEmail'];
  }
  public get platformTypeControl(): AbstractControl {
    return this.myForm.controls['platformType'];
  }
  public get platformStatusControl(): AbstractControl {
    return this.myForm.controls['status'];
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationNm: ['', [Validators.required]],
      organizationCd: ['', [Validators.required]],
      primaryNoticeOfTakedownContactEmail: ['', [Validators.required, Validators.email]],
      primaryTakedownRequestContactEmail: ['', [Validators.required, Validators.email]],
      secondaryNoticeOfTakedownContactEmail: ['', [Validators.email]],
      secondaryTakedownRequestContactEmail: ['', [Validators.email]],
      platformType: ['', [Validators.required]],
      status: [{ value: true, disabled: true }, []],
    });
  }
}
