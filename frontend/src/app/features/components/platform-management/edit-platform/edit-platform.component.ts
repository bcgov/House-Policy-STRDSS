import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DropdownOption } from '../../../../common/models/dropdown-option';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { OrganizationService } from '../../../../common/services/organization.service';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { Platform, UpdatePlatform } from '../../../../common/models/platform';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-platform',
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
  templateUrl: './edit-platform.component.html',
  styleUrl: './edit-platform.component.scss'
})
export class EditPlatformComponent implements OnInit {
  myForm!: FormGroup;
  platformTypes = new Array<DropdownOption>();
  id!: any;
  platform!: Platform;

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
    this.init();
  }

  onSave(): void {
    const platformRaw = this.myForm.getRawValue();

    const platform: UpdatePlatform = {
      isActive: platformRaw.isActive,
      organizationNm: platformRaw.organizationNm,
      platformType: platformRaw.platformType,
      primaryNoticeOfTakedownContactEmail: platformRaw.primaryNoticeOfTakedownContactEmail,
      primaryTakedownRequestContactEmail: platformRaw.primaryTakedownRequestContactEmail,
      secondaryNoticeOfTakedownContactEmail: platformRaw.secondaryNoticeOfTakedownContactEmail,
      secondaryTakedownRequestContactEmail: platformRaw.secondaryTakedownRequestContactEmail,
      updDtm: this.platform.updDtm,
    };
    this.loaderService.loadingStart();

    this.orgService.editPlatform(this.platform.organizationId, platform).subscribe({
      next: (_) => {
        this.messageHandlerService.showSuccess('The platform has been updated successfully')
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
    return this.myForm.controls['isActive'];
  }

  private init(): void {
    this.loaderService.loadingStart();

    this.orgService.getPlatformTypes().subscribe((types) => {
      this.platformTypes = types;
    });

    this.orgService.getPlatform(this.id)
      .subscribe({
        next: (platform) => {
          this.platform = platform as Platform;
          this.initForm();
        },
        complete: () => {
          this.loaderService.loadingEnd();
        },
      });
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationNm: [this.platform.organizationNm, [Validators.required]],
      organizationCd: [{ value: this.platform.organizationCd, disabled: true }, [Validators.required]],
      primaryNoticeOfTakedownContactEmail: [this.platform.primaryNoticeOfTakedownContactEmail, [Validators.required, Validators.email]],
      primaryTakedownRequestContactEmail: [this.platform.primaryTakedownRequestContactEmail, [Validators.required, Validators.email]],
      secondaryNoticeOfTakedownContactEmail: [this.platform.secondaryNoticeOfTakedownContactEmail, [Validators.email]],
      secondaryTakedownRequestContactEmail: [this.platform.secondaryTakedownRequestContactEmail, [Validators.email]],
      platformType: [this.platform.platformType, [Validators.required]],
      isActive: [!!this.platform.isActive, [Validators.required]],
    });
  }
}
