import { Component } from '@angular/core';
import { ListboxModule } from 'primeng/listbox';
import { UserRole } from '../../../../common/models/user-role';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
export class RoleDetailsComponent {
  permissions = new Array<any>();
  selectedPermissions = new Array<any>();
  isEdit = false;
  role!: UserRole;


  onSelectAllChange(data: any): void {

  }

  onChange(data: any): void {

  }
}
