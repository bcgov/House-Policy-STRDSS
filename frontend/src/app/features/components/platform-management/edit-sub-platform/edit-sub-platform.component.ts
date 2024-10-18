import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';
import { OrganizationService } from '../../../../common/services/organization.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Platform, SubPlatform, UpdatePlatform, UpdateSubPlatform } from '../../../../common/models/platform';

@Component({
  selector: 'app-edit-sub-platform',
  standalone: true,
  imports: [CommonModule,
    InputTextModule,
    DropdownModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    RadioButtonModule,
  ],
  templateUrl: './edit-sub-platform.component.html',
  styleUrl: './edit-sub-platform.component.scss'
})
export class EditSubPlatformComponent implements OnInit {
  myForm!: FormGroup;
  id!: any;
  platform!: SubPlatform;

  constructor(
    private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
    private orgService: OrganizationService,
    private route: ActivatedRoute,
    private router: Router,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];

    this.init();
  }

  onSave(): void {
    const platformRaw = this.myForm.getRawValue();
    const platform: UpdateSubPlatform = {
      isActive: platformRaw.isActive,
      organizationNm: platformRaw.organizationNm,
      managingOrganizationId: this.platform.managingOrganizationId,
      updDtm: this.platform.updDtm,
    };

    this.loaderService.loadingStart();

    this.orgService.editSubPlatform(this.platform.organizationId, platform).subscribe({
      next: () => {
        this.messageHandlerService.showSuccess('Sub platform has been updated successfully')
        this.onCancel();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      },
    });
  }

  onCancel(): void {
    this.router.navigateByUrl(`/platform/${this.platform.managingOrganizationId}`);
  }

  public get organizationNmControl(): AbstractControl {
    return this.myForm.controls['organizationNm'];
  }
  public get organizationCdControl(): AbstractControl {
    return this.myForm.controls['organizationCd'];
  }
  public get platformStatusControl(): AbstractControl {
    return this.myForm.controls['isActive'];
  }

  private init(): void {
    this.loaderService.loadingStart();

    this.orgService.getPlatform(this.id).subscribe({
      next: (res) => {
        this.platform = res as SubPlatform;
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
      isActive: [!!this.platform.isActive, [Validators.required]],
    });

    this.cd.detectChanges();
  }
}
