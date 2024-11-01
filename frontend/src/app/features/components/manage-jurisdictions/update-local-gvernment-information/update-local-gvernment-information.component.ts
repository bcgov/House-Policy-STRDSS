import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownOption } from '../../../../common/models/dropdown-option';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrganizationService } from '../../../../common/services/organization.service';
import { LocalGovernment, LocalGovernmentUpdate } from '../../../../common/models/jurisdiction';

@Component({
  selector: 'app-update-local-gvernment-information',
  standalone: true,
  imports: [
    CommonModule,
    InputTextModule,
    DropdownModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
  ],
  templateUrl: './update-local-gvernment-information.component.html',
  styleUrl: './update-local-gvernment-information.component.scss'
})
export class UpdateLocalGvernmentInformationComponent implements OnInit {
  myForm!: FormGroup;
  lgTypes = new Array<DropdownOption>();
  id!: any;
  lg!: LocalGovernment;

  constructor(
    private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
    private route: ActivatedRoute,
    private router: Router,
    private orgService: OrganizationService,
    private cd: ChangeDetectorRef,
  ) { }

  public get organizationNmControl(): AbstractControl {
    return this.myForm.controls['organizationNm'];
  }
  public get organizationCdControl(): AbstractControl {
    return this.myForm.controls['organizationCd'];
  }
  public get localGovernmentTypeControl(): AbstractControl {
    return this.myForm.controls['localGovernmentType'];
  }
  public get businessLicenceFormatTxtControl(): AbstractControl {
    return this.myForm.controls['businessLicenceFormatTxt'];
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.init();
  }

  onSave(): void {
    const formValue: {
      organizationNm: string,
      organizationCd: string,
      localGovernmentType: string,
      businessLicenceFormatTxt: string;
    } = this.myForm.getRawValue();

    const lgUpdate: LocalGovernmentUpdate = {
      updDtm: this.lg.updDtm,
      organizationId: this.lg.organizationId,
      businessLicenceFormatTxt: formValue.businessLicenceFormatTxt,
      localGovernmentType: formValue.localGovernmentType,
      organizationNm: formValue.organizationNm,
    };

    this.loaderService.loadingStart();
    this.orgService.updateLg(lgUpdate).subscribe({
      next: () => {
        this.messageHandlerService.showSuccess('Local Government has been successfully updated');
        this.onCancel();
        this.loaderService.loadingEnd();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      },
    });
  }

  onCancel(): void {
    this.router.navigateByUrl(`/manage-jurisdictions`);
  }

  private init(): void {
    this.loaderService.loadingStart();

    this.orgService.getLocalGovTypes().subscribe((types) => {
      this.lgTypes = types;
      this.cd.detectChanges();
    });

    this.orgService.getLg(this.id)
      .subscribe({
        next: (lg) => {
          this.lg = lg as LocalGovernment;
          this.initForm();
        },
        complete: () => {
          this.loaderService.loadingEnd();
        },
      });
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationNm: [this.lg.organizationNm, [Validators.required]],
      organizationCd: [{ value: this.lg.organizationCd, disabled: true }, [Validators.required]],
      localGovernmentType: [this.lg.localGovernmentType, [Validators.required]],
      businessLicenceFormatTxt: [this.lg.businessLicenceFormatTxt],
    });

    this.cd.detectChanges();
  }
}
