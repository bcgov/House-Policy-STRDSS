import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Dropdown, DropdownModule } from 'primeng/dropdown';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RequestAccessService } from '../../../common/services/request-access.service';
import { AccessRequestTableItem } from '../../../common/models/access-request-table-item';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { DialogModule } from 'primeng/dialog';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DateFormatPipe } from '../../../common/pipes/date-format.pipe';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { UserDataService } from '../../../common/services/user-data.service';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

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
    InputTextModule,
    ToastModule,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  @ViewChild("paginator") paginator!: Paginator;

  statuses = new Array<DropdownOption>();
  organizationTypes = new Array<DropdownOption>();
  organizations = new Array<DropdownOption>();
  filteredOrganizations = new Array<DropdownOption>();
  organizationDropdown = new Array<DropdownOption>();

  accessRequests = new Array<AccessRequestTableItem>();
  currentPage!: PagingResponsePageInfo;

  searchParams = {
    searchStatus: '',
    searchOrganization: null,
    searchTerm: '',
  }

  disableApproveButton = false;
  showApprovePopup = false;
  showRejectPopup = false;

  currentTableItem!: AccessRequestTableItem;
  currentOrganizationSelected: DropdownOption | undefined;
  currentOrganizationTypeSelected: DropdownOption | undefined;

  myForm!: FormGroup;

  constructor(
    private requestAccessService: RequestAccessService,
    private userDataService: UserDataService,
    private fb: FormBuilder,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.initData();
  }

  onSearchModelChanged(): void {
    if (this.paginator) {
      this.paginator.changePage(0);

      if (this.paginator.empty()) {
        this.getUsers();
      }
    }
  }

  onApprovePopup(accessRequest: AccessRequestTableItem): void {
    this.disableApproveButton = false;
    this.currentTableItem = accessRequest;
    this.showApprovePopup = true;
  }

  onRejectPopup(accessRequest: AccessRequestTableItem): void {
    this.currentTableItem = accessRequest;
    this.showRejectPopup = true;
  }

  onPageChange(pagingEvent: any): void {
    this.getUsers(pagingEvent.page + 1);
  }

  orgTypeChanged(orgType: any): void {
    this.filteredOrganizations = this.organizations.filter(org => (org as any)['organizationType'] === orgType);
  }

  onApprove(_orgTypeIdElem: Dropdown, orgId: Dropdown): void {
    this.disableApproveButton = true;

    const model = {
      userIdentityId: this.currentTableItem.userIdentityId,
      representedByOrganizationId: orgId.value,
      isEnabled: true,
      updDtm: this.currentTableItem.updDtm,
    };

    this.currentOrganizationSelected = undefined;
    this.currentOrganizationTypeSelected = undefined;

    this.loaderService.loadingStart();
    this.requestAccessService.approveAccessRequest(model).subscribe({
      next: () => {
        this.getUsers();
        this.onPopupClose();
      },
      error: (msg) => {
        if (msg.error.status === 422) {
          this.handleConcurrencyError(msg);
        } else {
          this.showErrorToast('Error', 'Unable to change user\'s access status. Check console for additional details')
        }
        console.error(msg);
        this.onPopupClose();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  onReject(): void {
    const model = {
      userIdentityId: this.currentTableItem.userIdentityId,
      updDtm: this.currentTableItem.updDtm,
    };
    this.loaderService.loadingStart();
    this.requestAccessService.denyAccessRequest(model).subscribe({
      next: () => {
        this.getUsers();
        this.onPopupClose();
      },
      error: (msg) => {
        if (msg.error.status === 422) {
          this.handleConcurrencyError(msg);
        }
        else {
          this.showErrorToast('Error', 'Unable to change user\'s access status. Check console for additional details')
        }
        console.error(msg);
        this.onPopupClose();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  onPopupClose(): void {
    this.currentOrganizationSelected = undefined;
    this.currentOrganizationTypeSelected = undefined;
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
        this.loaderService.loadingStart();
        this.userDataService.updateIsEnabled(accessRequest.userIdentityId, !accessRequest.isEnabled, accessRequest.updDtm).subscribe({
          next: () => {
            this.getUsers();
          },
          error: (msg) => {
            if (msg.error.status === 422) {
              this.handleConcurrencyError(msg);
            }
            else {
              this.showErrorToast('Error', 'Unable to change user\'s active status. Check console for additional details')
            }
            console.error(msg);
          },
          complete: () => {
            this.loaderService.loadingEnd();
          }
        })
        accessRequest.isEnabled = !accessRequest.isEnabled;
      },
      reject: () => {
        accessRequest.isEnabled = accessRequest.isEnabled;
      }, defaultFocus: 'reject'
    });
  }

  private handleConcurrencyError(errorMsg: any): void {
    let details = `${errorMsg.error.errors.entity[0]} Instance: ${errorMsg.error.instance}`;
    this.showErrorToast(errorMsg.error.title, details);
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      userIdentityId: [0, Validators.required],
      representedByOrganizationId: [0, Validators.required],
    });
  }

  private initData(): void {
    this.loaderService.loadingStart();
    this.userDataService.getStatuses().subscribe({
      next: (data: Array<DropdownOption>) => {
        this.statuses = [{ label: 'All', value: '' }, ...data];
      },
      error: (error) => {
        this.showErrorToast('Error', 'Unable to retrieve Statuses. Check console for additional details')
        console.error(error);
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });

    this.loaderService.loadingStart();
    this.requestAccessService.getOrganizations().subscribe({
      next: (data) => {
        this.organizations = data;
        this.filteredOrganizations = data;
        this.organizationDropdown = [{ label: 'All', value: '' }, ...data];
      },
      error: (error: any) => {
        this.showErrorToast('Error', 'Unable to retrieve Organizations. Check console for additional details')
        console.error(error);
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });

    this.loaderService.loadingStart();
    this.requestAccessService.getOrganizationTypes().subscribe({
      next: (data) => {
        this.organizationTypes = data;
      },
      error: (error: any) => {
        this.showErrorToast('Error', 'Unable to retrieve Organization Types. Check console for additional details')
        console.error(error);
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });

    this.getUsers();
  }

  private getUsers(selectedPageNumber?: number): void {
    const status = this.searchParams.searchStatus;
    const search = this.searchParams.searchTerm;
    const organizationId = this.searchParams.searchOrganization;
    const pageSize = this.currentPage?.pageSize || 10;
    const pageNumber = selectedPageNumber ?? (this.currentPage?.pageNumber || 0);
    const orderBy = '';
    const direction = 'desc';
    this.loaderService.loadingStart();

    this.userDataService.getUsers(status, search, organizationId, pageSize, pageNumber, orderBy, direction).subscribe({
      next: (response: PagingResponse<AccessRequestTableItem>) => {
        this.accessRequests = response.sourceList;
        this.currentPage = response.pageInfo;

      },
      error: (error: any) => {
        this.showErrorToast('Error', 'Unable to retrieve users. Check console for additional details')
        console.error(error);

      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }

  private showErrorToast(title: string, errorMsg: string) {
    this.messageService.add({ severity: 'error', summary: title, detail: errorMsg, sticky: true });
  }
}
