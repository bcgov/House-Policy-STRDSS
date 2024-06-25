import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { UserPermission, UserPermissionSelectable, UserRole } from '../../../../common/models/user-role';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RolesService } from '../../../../common/services/roles.service';
import { ActivatedRoute, Router } from '@angular/router';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { forkJoin } from 'rxjs';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { ErrorHandlingService } from '../../../../common/services/error-handling.service';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [
    CheckboxModule,
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    ReactiveFormsModule,
    ConfirmDialogModule,
  ],
  providers: [ConfirmationService],
  templateUrl: './role-details.component.html',
  styleUrl: './role-details.component.scss'
})
export class RoleDetailsComponent implements OnInit {
  permissions = new Array<UserPermissionSelectable>();
  selectedPermissions = new Array<UserPermission>();
  isEdit = false;
  isCreate = false;
  role!: UserRole | null;
  id!: string;
  editRoleName = '';
  editRoleDescription = '';

  constructor(
    private route: ActivatedRoute,
    private roleService: RolesService,
    private loaderService: GlobalLoaderService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private errorHandlingService: ErrorHandlingService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.id = this.route.snapshot.params['id'];

    if (!!this.id) {
      this.initEdit();
    } else {
      this.initCreate();
    }
  }

  onCreate(): void {
    this.loaderService.loadingStart();
    this.role = {
      userRoleCd: this.editRoleName,
      userRoleDsc: '',
      userRoleNm: this.editRoleDescription,
      isReferenced: false,
      permissions: this.permissions
        .filter((perm) => (perm.selected))
        .map((perm) => ({ userPrivilegeCd: perm.userPrivilegeCd, userPrivilegeNm: perm.userPrivilegeNm })),
    };

    this.roleService.createRole(this.role).subscribe({
      next: (_) => {
        this.errorHandlingService.showSuccess(`Role [${this.role?.userRoleCd}] has been successfully created`);
        this.loaderService.loadingEnd();
        this.router.navigate([`role/${this.role?.userRoleCd}`]);
      },
      complete: () => {
        this.role = null;
        this.cd.detectChanges();
        this.loaderService.loadingEnd();
      },
    });
  }

  onCreateCancel(): void {
    if (this.editRoleDescription || this.editRoleName || this.permissions.some((item) => (item.selected))) {
      this.confirmationService.confirm({
        message: 'Are you sure you want to leave this page? All unsaved changes will disappear.',
        header: 'Unsaved Changes',
        acceptLabel: 'Leave this Page',
        rejectLabel: 'Return',
        acceptIcon: 'null',
        rejectIcon: 'null',
        acceptButtonStyleClass: 'outline-btn',

        accept: () => {
          this.router.navigate(['roles']);
        },
      });
    }
    else {
      this.router.navigate(['roles']);
    }
  }

  onEditSave(): void {
    this.loaderService.loadingStart();
    if (this.role) {
      this.role.userRoleNm = this.editRoleDescription;
      this.role.permissions = this.permissions.filter((perm) => (perm.selected)).map((perm) => ({ userPrivilegeCd: perm.userPrivilegeCd, userPrivilegeNm: perm.userPrivilegeNm }))
      this.roleService.updateRole(this.role).subscribe({
        next: (_) => {
          this.errorHandlingService.showSuccess(`Role [${this.role?.userRoleCd}] has been successfully updated`);
        },
        complete: () => {
          this.cd.detectChanges();
          this.loaderService.loadingEnd();
        },
      });
      this.isEdit = false;
    }
  }

  onEditCancel(): void {
    this.editRoleDescription = this.role?.userRoleNm || '';
    this.permissions = this.permissions.map((perm) => (
      {
        userPrivilegeCd: perm.userPrivilegeCd,
        userPrivilegeNm: perm.userPrivilegeNm,
        selected: !!this.selectedPermissions.find((selPerm) => {
          return (perm.userPrivilegeCd === selPerm.userPrivilegeCd && perm.userPrivilegeNm === selPerm.userPrivilegeNm)
        })
      }
    ));
    this.isEdit = false;
  }

  onEdit(): void {
    this.isEdit = true;
  }

  onDeleteRole(): void {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this role? You cannot undo this decision.',
      header: 'Delete This Role?',
      acceptLabel: 'Yes, Delete this Role',
      rejectLabel: 'Return',
      acceptIcon: 'null',
      rejectIcon: 'null',
      acceptButtonStyleClass: 'outline-btn',

      accept: () => {
        if (this.role) {
          this.loaderService.loadingStart();
          this.roleService.deleteRole(this.role.userRoleCd).subscribe({
            next: () => {
              this.loaderService.loadingEnd();
              this.router.navigate(['roles']);
            }, complete: () => {
              this.cd.detectChanges();
              this.loaderService.loadingEnd();
            }
          });
        }
      },
    });
  }

  private initCreate(): void {
    this.loaderService.loadingStart();

    this.roleService.getPermissions().subscribe({
      next: (permissions) => {
        this.permissions = permissions.map((perm) => ({ ...perm, selected: false }));
        this.isCreate = true;
      }, complete: () => {
        this.cd.detectChanges();
        this.loaderService.loadingEnd();
      }
    })
  }

  private initEdit(): void {
    const getRoleById = this.roleService.getRole(this.id);
    const getPermissions = this.roleService.getPermissions();

    forkJoin([getRoleById, getPermissions])
      .subscribe({
        next: ([role, permissions]) => {
          this.role = role;
          this.selectedPermissions = role.permissions;
          this.editRoleDescription = role.userRoleNm;
          this.editRoleName = role.userRoleCd;

          this.permissions = permissions.map((perm) => (
            {
              userPrivilegeCd: perm.userPrivilegeCd,
              userPrivilegeNm: perm.userPrivilegeNm,

              selected: !!this.selectedPermissions.find((selPerm) => {
                return (perm.userPrivilegeCd === selPerm.userPrivilegeCd && perm.userPrivilegeNm === selPerm.userPrivilegeNm)
              })
            }
          ));
        },
        complete: () => {
          this.cd.detectChanges();
          this.loaderService.loadingEnd();
        }
      });
  }
}
