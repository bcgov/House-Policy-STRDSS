import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrganizationService } from '../../../../common/services/organization.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { DropdownOption } from '../../../../common/models/dropdown-option';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-add-sub-platform',
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
  templateUrl: './add-sub-platform.component.html',
  styleUrl: './add-sub-platform.component.scss'
})
export class AddSubPlatformComponent implements OnInit {
  myForm!: FormGroup;
  id!: any;

  constructor(
    private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
    private route: ActivatedRoute,
    private router: Router,
    private orgService: OrganizationService,
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];

    this.initForm();
  }

  onSave(): void {
    const platform = this.myForm.getRawValue();
    platform.managingOrganizationId = this.id;

    this.loaderService.loadingStart();
    this.orgService.addSubPlatform(platform).subscribe({
      next: (_) => {
        this.messageHandlerService.showSuccess('New subsidiary platform has been added successfully')
        this.onCancel();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      },
    });
  }

  onCancel(): void {
    this.router.navigateByUrl(`/platform/${this.id}`);
  }

  public get organizationNmControl(): AbstractControl {
    return this.myForm.controls['organizationNm'];
  }
  public get organizationCdControl(): AbstractControl {
    return this.myForm.controls['organizationCd'];
  }
  public get platformStatusControl(): AbstractControl {
    return this.myForm.controls['status'];
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

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationNm: ['', [Validators.required]],
      organizationCd: ['', [Validators.required]],
      primaryNoticeOfTakedownContactEmail: ['', [Validators.required, Validators.email]],
      primaryTakedownRequestContactEmail: ['', [Validators.required, Validators.email]],
      secondaryNoticeOfTakedownContactEmail: ['', [Validators.email]],
      secondaryTakedownRequestContactEmail: ['', [Validators.email]],
      status: [{ value: true, disabled: true }, []],
    });
  }
}
