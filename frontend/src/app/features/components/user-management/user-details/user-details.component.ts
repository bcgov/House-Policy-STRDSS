import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { UserDataService } from '../../../../common/services/user-data.service';
import { UserDetails } from '../../../../common/models/user';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { RolesService } from '../../../../common/services/roles.service';
import { forkJoin } from 'rxjs';
import { DropdownOption, DropdownOptionSelectable } from '../../../../common/models/dropdown-option';
import { FormsModule } from '@angular/forms';
import { MultiSelectModule } from 'primeng/multiselect';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';
import { OrganizationService } from '../../../../common/services/organization.service';

@Component({
  selector: 'app-user-details',
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
  ],
  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss'
})
export class UserDetailsComponent implements OnInit {
  id!: number;
  user!: UserDetails;
  roles = new Array<DropdownOptionSelectable>();
  tags = new Array<string>();
  orgTypes = new Array<DropdownOption>();
  organizationsRaw = new Array<DropdownOption>();
  organizations = new Array<DropdownOption>();
  isEdit = false;
  state!: 'Active' | 'Disabled';

  selectedOrg = '';
  selectedOrgType = '';

  constructor(
    private userService: UserDataService,
    private rolesService: RolesService,
    private errorService: ErrorHandlingService,
    private organizationsService: OrganizationService,
    private route: ActivatedRoute,
    private router: Router,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.params['id']);

    if (isNaN(this.id)) {
      this.errorService.showError('Invalid User ID: User ID must consist of numbers only.');
      this.router.navigate(['/user-management']);
    }

    this.init();
  }

  onUpdate(): void {
    this.isEdit = true;
  }

  onCancel(): void {
    if (this.isEdit) {
      this.isEdit = false;
      this.init();
    } else {
      this.router.navigate(['/user-management']);
    }
  }

  onSave(): void {
    this.userService.updateUser({
      roleCds: this.user.roleCds,
      updDtm: this.user.updDtm,
      userIdentityId: this.user.userIdentityId,
      isEnabled: this.state === 'Active',
      representedByOrganizationId: this.selectedOrg
    }).subscribe({
      next: (_) => {
        this.init();
        this.isEdit = false;
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  onOrgTypeChanged(orgType: any): void {
    this.organizations = this.organizationsRaw.filter(x =>
      (x as any)['organizationType'] === orgType.value)
  }

  onSelectedRolesChanged(_: any): void {
    this.roles = this.roles.map((role) => ({ ...role, selected: this.user.roleCds.some(x => role.value === x) }));
    this.tags = this.roles.filter(x => x.selected).map(x => x.label);

    this.cd.detectChanges();
  }

  private init(): void {
    this.loaderService.loadingStart();

    forkJoin([
      this.userService.getUserById(this.id),
      this.rolesService.getRoles(),
      this.organizationsService.getOrganizationTypes(),
      this.organizationsService.getOrganizations(),
    ]).subscribe({
      next: ([user, roles, orgTypes, organizations]) => {
        this.user = user;
        this.orgTypes = orgTypes.sort((a, b) => this.sortHandler(a.label, b.label));
        this.organizations = organizations.sort((a, b) => this.sortHandler(a.label, b.label));
        this.organizationsRaw = organizations.sort((a, b) => this.sortHandler(a.label, b.label));
        this.selectedOrg = this.organizationsRaw.find(x => (x as any)['organizationCd'] === this.user.representedByOrganization.organizationCd)?.value || '';
        this.selectedOrgType = this.user.representedByOrganization.organizationType;
        this.roles = roles.map((role) => ({ label: role.userRoleNm, value: role.userRoleCd, selected: user.roleCds.some(x => role.userRoleCd === x) }));
        this.state = this.user.isEnabled ? 'Active' : 'Disabled';
        this.tags = this.roles.filter(x => x.selected).map(x => x.label);
        this.cd.detectChanges();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  private sortHandler(a: string, b: string): number {
    if (a > b) return 1;
    if (a < b) return -1;
    return 0
  }
}
