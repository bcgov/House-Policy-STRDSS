import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { UploadFileComponent } from '../../../common/components/upload-file/upload-file.component';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { ToastModule } from 'primeng/toast';
import { environment } from '../../../../environments/environment';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { TableModule } from 'primeng/table';
import { BusinessLicenceService } from '../../../common/services/business-licence.service';
import { DialogModule } from 'primeng/dialog';
import { UserDataService } from '../../../common/services/user-data.service';
import { User } from '../../../common/models/user';
import { DropdownModule } from 'primeng/dropdown';
import { DropdownOptionOrganization } from '../../../common/models/dropdown-option';
import { RequestAccessService } from '../../../common/services/request-access.service';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { PagingResponsePageInfo } from '../../../common/models/paging-response';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-upload-business-license',
  standalone: true,
  imports: [
    UploadFileComponent,
    ButtonModule,
    DropdownModule,
    CommonModule,
    FormsModule,
    ToastModule,
    UploadFileComponent,
    TableModule,
    PaginatorModule,
    DialogModule,
  ],
  templateUrl: './upload-business-license.component.html',
  styleUrl: './upload-business-license.component.scss'
})
export class UploadBusinessLicenseComponent implements OnInit {
  maxFileSize = Number(environment.BUISINESS_LICENCE_MAX_SIZE) * 1024 * 1024;
  selectedFile!: File | null;
  isFileUploadDisabled = false;
  isSizeLimitExceeded = false;
  isExtensionInvalid = false;
  updateFileRef!: UploadFileComponent;
  isUploadVisible = false;
  uploadHistory = new Array<any>();
  fileName!: string
  isCEU!: boolean;
  currentUser!: User;
  groupedCommunities = new Array();
  jurisdictionId!: number;
  currentPage!: PagingResponsePageInfo;
  sort!: { prop: string, dir: 'asc' | 'desc' }
  uploadRef!: UploadFileComponent;

  constructor(private BLService: BusinessLicenceService,
    private loaderService: GlobalLoaderService,
    private userDataService: UserDataService,
    private messageService: MessageService,
    private requestAccessService: RequestAccessService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.init();
    this.getOrganizations();
  }

  onUploadFile(): void {
    this.loaderService.loadingStart();
    this.onUploadClose();

    this.BLService.uploadFile(this.selectedFile, this.currentUser.organizationId).subscribe({
      next: () => {
        this.init();
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File has been uploaded successfully' });
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.onClear();
        this.cd.detectChanges();
      },
    });
  }

  onFileSelected(files: any, uploadRef: UploadFileComponent): void {
    if (uploadRef) {
      this.uploadRef = uploadRef;
    }

    if (!!files) {
      this.selectedFile = files[0];
      this.isFileUploadDisabled = !!files.length;
    }
  }

  onUploadOpen(): void {
    this.isUploadVisible = true;
  }

  onUploadClose(): void {
    this.isUploadVisible = false;
  }

  onCancelUpload(): void {
    this.selectedFile = null;
    this.onUploadClose();
  }

  onSort(prop: string): void {
    if (this.sort) {
      if (this.sort.prop === prop) {
        this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
      } else {
        this.sort.prop = prop;
        this.sort.dir = 'asc';
      }
    }
    else {
      this.sort = { prop: prop, dir: 'asc' };
    }

    this.getHistoryRecords();
  }

  onClear(): void {
    this.selectedFile = null;
    this.isFileUploadDisabled = false;
  }

  onJurisdictionChanged(_: any): void {
    this.loaderService.loadingStart();
    this.getHistoryRecords();
  }

  onPageChange(e: any): void {
    console.log(e.page + 1);
    this.getHistoryRecords(e.page + 1);
  }

  private getHistoryRecords(selectedPageNumber: number = 1): void {
    this.loaderService.loadingStart();
    this.BLService.getUploadHistory(
      selectedPageNumber,
      10,
      this.sort?.prop,
      this.sort?.dir,
      this.jurisdictionId,
    ).subscribe({
      next: (records) => {
        this.currentPage = records.pageInfo;
        this.uploadHistory = records.sourceList;
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }

  private init(): void {
    this.userDataService.getCurrentUser().subscribe({
      next: (x) => {
        this.currentUser = x;
        this.isCEU = this.userDataService.currentUser.organizationType === 'BCGov';
        this.cd.detectChanges();
        this.getHistoryRecords();
      }
    });
  }

  private getOrganizations(): void {
    this.requestAccessService.getOrganizations('LG').subscribe({
      next: (orgs) => {
        const communities = orgs.map((org: DropdownOptionOrganization) =>
          ({ label: org.label, value: org.value, localGovernmentType: org.localGovernmentType || 'Other' }));

        const groupedData: Array<any> = communities.reduce((acc: any, curr: any) => {
          const existingGroup = acc.find((group: any) => group.value === curr.localGovernmentType);
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

        const municipality = groupedData.filter(x => x.label === 'Municipality')[0];
        const regional = groupedData.filter(x => x.label === 'Regional District Electoral Area')[0];
        const other = groupedData.filter(x => x.label === 'Other')[0];
        const firstNations = groupedData.filter(x => x.label === 'First Nations Community')[0];
        const uncategorized = groupedData.filter(x =>
          x.label !== 'Municipality' &&
          x.label !== 'Regional District Electoral Area' &&
          x.label !== 'Other' &&
          x.label !== 'First Nations Community'
        );

        const sorted = [];

        if (municipality)
          sorted.push(municipality);
        if (regional)
          sorted.push(regional);
        if (other)
          sorted.push(other);
        if (firstNations)
          sorted.push(firstNations);
        if (uncategorized.length)
          sorted.push(...uncategorized);

        this.groupedCommunities = sorted;
      }
    });
  }
}
