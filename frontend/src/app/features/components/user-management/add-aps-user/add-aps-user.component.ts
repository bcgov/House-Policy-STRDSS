import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { RadioButtonModule } from 'primeng/radiobutton';
import { TagModule } from 'primeng/tag';
import { DropdownOptionSelectable, DropdownOption } from '../../../../common/models/dropdown-option';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { RequestAccessService } from '../../../../common/services/request-access.service';
import { RolesService } from '../../../../common/services/roles.service';
import { UserDataService } from '../../../../common/services/user-data.service';
import { forkJoin } from 'rxjs';
import { ApsUser } from '../../../../common/models/aps-user';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-add-aps-user',
  standalone: true,
  imports: [
    CommonModule,
    TagModule,
    ButtonModule,
    DropdownModule,
    FormsModule,
    RouterModule,
    MultiSelectModule,
    RadioButtonModule,
    InputTextModule,
  ],
  templateUrl: './add-aps-user.component.html',
  styleUrl: './add-aps-user.component.scss'
})
export class AddApsUserComponent implements OnInit {
  user: ApsUser = { displayNm: '', representedByOrganizationId: 0, isEnabled: true, roleCds: [], }

  roles = new Array<DropdownOptionSelectable>();
  tags = new Array<string>();
  orgTypes = new Array<DropdownOption>();
  organizationsRaw = new Array<DropdownOption>();
  organizations = new Array<DropdownOption>();

  state: 'Active' | 'Disabled' = 'Active';
  selectedOrg = 0;
  selectedOrgType = '';

  constructor(
    private userService: UserDataService,
    private rolesService: RolesService,
    private errorService: ErrorHandlingService,
    private requestAccessService: RequestAccessService,
    private router: Router,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.init();
  }

  onCancel(): void {
    this.router.navigate(['/user-management']);
  }

  onOrgTypeChanged(orgType: any): void {
    this.organizations = this.organizationsRaw.filter(x =>
      (x as any)['organizationType'] === orgType.value);
  }

  onSelectedRolesChanged(_: any): void {
    this.roles = this.roles.map((role) => ({ ...role, selected: this.user.roleCds.some(x => role.value === x) }));
    this.tags = this.roles.filter(x => x.selected).map(x => x.label);

    this.cd.detectChanges();
  }

  onSave(): void {
    this.loaderService.loadingStart();
    this.user.representedByOrganizationId = this.selectedOrg;
    this.user.isEnabled = this.state === 'Active'
    this.userService.createApsUser(this.user).subscribe({
      next: (response: any) => {
        this.errorService.showSuccess('APS user created successfully');
        this.router.navigateByUrl(`/user/${response}`);
      },
      complete: () => {
        this.cd.detectChanges();
        this.loaderService.loadingEnd();
      },
    });
  }

  private init(): void {
    this.loaderService.loadingStart();

    forkJoin([
      this.rolesService.getRoles(),
      this.requestAccessService.getOrganizationTypes(),
      this.requestAccessService.getOrganizations(),
    ]).subscribe({
      next: ([roles, orgTypes, organizations]) => {
        this.orgTypes = orgTypes.sort((a, b) => this.sortHandler(a.label, b.label));
        this.organizations = organizations.sort((a, b) => this.sortHandler(a.label, b.label));
        this.organizationsRaw = organizations.sort((a, b) => this.sortHandler(a.label, b.label));
        this.tags = this.roles.filter(x => x.selected).map(x => x.label);
        this.roles = roles.map((role) => ({ label: role.userRoleNm, value: role.userRoleCd, selected: false }));
        this.cd.detectChanges();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      },
    });
  }

  private sortHandler(a: string, b: string): number {
    if (a > b) return 1;
    if (a < b) return -1;
    return 0;
  }
}
