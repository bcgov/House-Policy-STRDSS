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


@Component({
  selector: 'app-access-request-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    DropdownModule,
    TableModule,
    ButtonModule,
    DialogModule,
    PaginatorModule,
  ],
  templateUrl: './access-request-list.component.html',
  styleUrl: './access-request-list.component.scss'
})
export class AccessRequestListComponent implements OnInit {
  statuses = new Array<DropdownOption>();
  organizationTypes = new Array<DropdownOption>();
  organizations = new Array<DropdownOption>();

  accessRequests = new Array<AccessRequestTableItem>();
  currentPage!: PagingResponsePageInfo;

  showApprovePopup = false;
  showRejectPopup = false;

  currentTableItem!: AccessRequestTableItem;

  myForm!: FormGroup;

  first = 0;
  total = 120;

  constructor(private requestAccessService: RequestAccessService, private fb: FormBuilder,) { }

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

  private initForm(): void {
    this.myForm = this.fb.group({
      userIdentityId: [0, Validators.required],
      representedByOrganizationId: [0, Validators.required],
    });
  }

  private initData(): void {
    this.statuses = [
      { label: 'Approved', value: 1 },
      { label: 'Denied', value: 2 },
      { label: 'Pending', value: 3 },
    ]

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

    this.requestAccessService.getAccessRequests({ pageNumber: 1, pageSize: 10 }).subscribe({
      next: (response: PagingResponse<AccessRequestTableItem>) => {
        this.accessRequests = response.sourceList;
        this.currentPage = response.pageInfo;
        console.log(response);
      },
      error: (error: any) => {
        console.log(error);
      }
    })
  }
}
