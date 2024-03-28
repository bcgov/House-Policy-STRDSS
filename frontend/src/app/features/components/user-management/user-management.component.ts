import { Component, OnInit } from '@angular/core';
import { DropdownModule } from 'primeng/dropdown';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RequestAccessService } from '../../../common/services/request-access.service';
import { AccessRequestTableItem } from '../../../common/models/access-request-table-item';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { DialogModule } from 'primeng/dialog';
import { PaginatorModule } from 'primeng/paginator';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DateFormatPipe } from '../../../common/pipes/date-format.pipe';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { UserDataService } from '../../../common/services/user-data.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    DropdownModule,
    TableModule,
    ButtonModule,
    DialogModule,
    PaginatorModule,
    DateFormatPipe,
    InputSwitchModule,
    ConfirmDialogModule,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  statuses = new Array<DropdownOption>();
  organizationTypes = new Array<DropdownOption>();
  organizations = new Array<DropdownOption>();

  accessRequests = new Array<AccessRequestTableItem>();
  currentPage!: PagingResponsePageInfo;
  currentStatus = '';

  showApprovePopup = false;
  showRejectPopup = false;

  currentTableItem!: AccessRequestTableItem;

  myForm!: FormGroup;

  first = 0;
  total = 120;

  constructor(private requestAccessService: RequestAccessService, private userDataService: UserDataService, private fb: FormBuilder, private confirmationService: ConfirmationService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.initForm();
    this.initData();
  }

  onFilterChanged(statusId: any): void {
    console.log('statusId', statusId);
  }

  onApprovePopup(accessRequest: AccessRequestTableItem): void {
    console.log('Approve', accessRequest);
    this.showApprovePopup = true;
  }

  onRejectPopup(accessRequest: AccessRequestTableItem): void {
    console.log('Reject', accessRequest);
    this.showRejectPopup = true;
  }

  onPageChange(pagingEvent: any): void {
    console.log('pagingEvent', pagingEvent);
  }

  onApprove(): void {
    this.currentTableItem.userIdentityId;
  }

  onReject(): void {

  }

  onPopupClose(): void {
    this.showApprovePopup = false;
    this.showRejectPopup = false;
  }

  onActivateDeactivateToggle(event: any, accessRequest: AccessRequestTableItem): void {
    if (accessRequest.accessRequestStatusCd === 'Requested') {
      return;
    }

    const user = `${accessRequest.givenNm} ${accessRequest.familyNm}`;
    const messageAction = `${accessRequest.isEnabled ? 'Deactivate' : 'Activate'}`;
    const titleAction = `${accessRequest.isEnabled ? 'Deactivating' : 'Activating'}`;
    const acceptButtonLabel = `${accessRequest.isEnabled ? 'Deactivate' : 'Activate'}`;
    const acceptButtonClass = `${!accessRequest.isEnabled || 'p-button-red'}`;

    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: `Are you sure that you want to ${messageAction} ${user}'s account?`,
      header: `${titleAction} User's Account`,
      icon: 'none',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-outlined',
      acceptButtonStyleClass: acceptButtonClass,
      acceptLabel: acceptButtonLabel,
      rejectLabel: 'Cancel',
      closeOnEscape: false,
      accept: () => {
        this.userDataService.updateIsEnabled(accessRequest.userIdentityId, !accessRequest.isEnabled, accessRequest.updDtm).subscribe({
          next: () => {
            this.getUsers();
          },
          error: (error) => {
            console.error(error);
          }
        })
        accessRequest.isEnabled = !accessRequest.isEnabled;
      },
      reject: () => {
        accessRequest.isEnabled = accessRequest.isEnabled;
      }, defaultFocus: 'reject'
    });
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      userIdentityId: [0, Validators.required],
      representedByOrganizationId: [0, Validators.required],
    });
  }

  private initData(): void {
    this.userDataService.getStatuses().subscribe({
      next: (data: Array<DropdownOption>) => {
        this.statuses = data;
      }
    })

    this.requestAccessService.getOrganizations().subscribe({
      next: (data) => {
        this.organizations = data;
      },
      error: (error: any) => {
        console.log(error);
      }
    })

    this.requestAccessService.getOrganizationTypes().subscribe({
      next: (data) => {
        this.organizationTypes = data;
      },
      error: (error: any) => {
        console.log(error);
      }
    })

    this.getUsers();
  }

  private getUsers(): void {
    this.requestAccessService.getAccessRequests({ pageNumber: 1, pageSize: 10 }).subscribe({
      next: (response: PagingResponse<AccessRequestTableItem>) => {
        this.accessRequests = response.sourceList;
        this.currentPage = response.pageInfo;
        console.log(response);
      },
      error: (error: any) => {
        console.error(error);
      }
    })
  }
}
