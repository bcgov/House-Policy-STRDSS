import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownOption, DropdownOptionOrganization } from '../../../../common/models/dropdown-option';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrganizationService } from '../../../../common/services/organization.service';
import { Jurisdiction, JurisdictionUpdate } from '../../../../common/models/jurisdiction';
import { RadioButtonModule } from 'primeng/radiobutton';

@Component({
  selector: 'app-update-jurisdiction-information',
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
  templateUrl: './update-jurisdiction-information.component.html',
  styleUrl: './update-jurisdiction-information.component.scss'
})
export class UpdateJurisdictionInformationComponent implements OnInit {
  myForm!: FormGroup;
  economicRegions = new Array<DropdownOption>();
  id!: any;
  jurisdiction!: Jurisdiction;
  groupedCommunities = new Array();

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
  public get shapeFileIdControl(): AbstractControl {
    return this.myForm.controls['shapeFileId'];
  }
  public get managingOrganizationIdControl(): AbstractControl {
    return this.myForm.controls['managingOrganizationId'];
  }
  public get isPrincipalResidenceRequiredControl(): AbstractControl {
    return this.myForm.controls['isPrincipalResidenceRequired'];
  }
  public get isStrProhibitedControl(): AbstractControl {
    return this.myForm.controls['isStrProhibited'];
  }
  public get isBusinessLicenceRequiredControl(): AbstractControl {
    return this.myForm.controls['isBusinessLicenceRequired'];
  }
  public get economicRegionDscControl(): AbstractControl {
    return this.myForm.controls['economicRegionDsc'];
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.init();
  }

  onSave(): void {
    const formValue: {
      organizationNm: string;
      managingOrganizationId: number;
      isPrincipalResidenceRequired: boolean;
      isStrProhibited: boolean;
      isBusinessLicenceRequired: boolean;
      economicRegionDsc: string;
    } = this.myForm.getRawValue();

    const jurisdictionUpdate: JurisdictionUpdate = {
      organizationId: this.jurisdiction.organizationId,
      economicRegionDsc: formValue.economicRegionDsc,
      isBusinessLicenceRequired: formValue.isBusinessLicenceRequired,
      isPrincipalResidenceRequired: formValue.isPrincipalResidenceRequired,
      isStrProhibited: formValue.isStrProhibited,
      managingOrganizationId: formValue.managingOrganizationId,
      updDtm: this.jurisdiction.updDtm,
    };

    this.loaderService.loadingStart();

    this.orgService.updateJurisdiction(jurisdictionUpdate).subscribe({
      next: () => {
        this.messageHandlerService.showSuccess('Jurisdiction has been successfully updated');
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

    this.orgService.getEconomicRegions().subscribe((economicRegions) => {
      this.economicRegions = economicRegions;
      this.cd.detectChanges();
    });

    this.getOrganizations();

    this.orgService.getJurisdiction(this.id)
      .subscribe({
        next: (lg) => {
          this.jurisdiction = lg as Jurisdiction;
          this.initForm();
        },
        complete: () => {
          this.loaderService.loadingEnd();
        },
      });
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      organizationNm: [{ value: this.jurisdiction.organizationNm, disabled: true }, [Validators.required]],
      shapeFileId: [{ value: this.jurisdiction.shapeFileId, disabled: true }, [Validators.required]],
      managingOrganizationId: [this.jurisdiction.managingOrganizationId, [Validators.required]],
      isPrincipalResidenceRequired: [this.jurisdiction.isPrincipalResidenceRequired || false, [Validators.required]],
      isStrProhibited: [this.jurisdiction.isStrProhibited || false, [Validators.required]],
      isBusinessLicenceRequired: [this.jurisdiction.isBusinessLicenceRequired || false, [Validators.required]],
      economicRegionDsc: [this.jurisdiction.economicRegionDsc, [Validators.required]],
    });

    this.cd.detectChanges();
  }

  private getOrganizations(): void {
    this.orgService.getOrganizations('LG').subscribe({
      next: (orgs) => {
        const communities = orgs.map((org: DropdownOptionOrganization) => ({
          label: org.label,
          value: org.value,
          localGovernmentType: org.localGovernmentType || 'Other',
        }));

        const groupedData: Array<any> = communities.reduce((acc: any, curr: any) => {
          const existingGroup = acc.find(
            (group: any) => group.value === curr.localGovernmentType,
          );
          if (existingGroup) {
            existingGroup.items.push({ label: curr.label, value: curr.value });
          } else {
            acc.push({
              label: curr.localGovernmentType,
              value: curr.localGovernmentType,
              items: [{ label: curr.label, value: curr.value }],
            });
          }

          return acc;
        }, []);
        const municipality = groupedData.filter((x) => x.label === 'Municipality')[0];
        const regional = groupedData.filter(
          (x) => x.label === 'Regional District Electoral Area',
        )[0];
        const other = groupedData.filter((x) => x.label === 'Other')[0];
        const firstNations = groupedData.filter(
          (x) => x.label === 'First Nations Community',
        )[0];
        const uncategorized = groupedData.filter(
          (x) =>
            x.label !== 'Municipality' &&
            x.label !== 'Regional District Electoral Area' &&
            x.label !== 'Other' &&
            x.label !== 'First Nations Community',
        );

        const sorted = [];

        if (municipality) sorted.push(municipality);
        if (regional) sorted.push(regional);
        if (other) sorted.push(other);
        if (firstNations) sorted.push(firstNations);
        if (uncategorized.length) sorted.push(...uncategorized);

        this.groupedCommunities = sorted;
        this.cd.detectChanges();
      },
    });
  }
}