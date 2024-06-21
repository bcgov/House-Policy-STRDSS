import { Component, OnInit } from '@angular/core';
import { ListboxModule } from 'primeng/listbox';
import { UserRole } from '../../../../common/models/user-role';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RolesService } from '../../../../common/services/roles.service';
import { ActivatedRoute } from '@angular/router';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { forkJoin, mergeMap } from 'rxjs';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [
    ListboxModule,
    CommonModule,
    FormsModule,
  ],
  templateUrl: './role-details.component.html',
  styleUrl: './role-details.component.scss'
})
export class RoleDetailsComponent implements OnInit {
  permissions = new Array<any>();
  selectedPermissions = new Array<any>();
  isEdit = false;
  role!: UserRole;
  id!: string;

  constructor(private route: ActivatedRoute, private roleService: RolesService, private loaderService: GlobalLoaderService) { }

  ngOnInit(): void {
    this.loaderService.loadingStart();
    this.id = this.route.snapshot.params['id'];


    const getRoleById = this.roleService.getRoleById(this.id)
    const getPermissions = this.roleService.getPermissions();

    forkJoin([getRoleById, getPermissions])
      .subscribe({
        next: ([role, permissions]) => {
          this.role = role;
          this.permissions = permissions;
          this.selectedPermissions = role.userPrivilegeCds;
        },
        complete: () => { this.loaderService.loadingEnd(); }
      });
  }

  onSelectAllChange(data: any): void { }

  onChange(data: any): void { }
}
