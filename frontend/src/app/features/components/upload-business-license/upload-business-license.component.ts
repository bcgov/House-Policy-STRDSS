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

@Component({
  selector: 'app-upload-business-license',
  standalone: true,
  imports: [
    UploadFileComponent,
    ButtonModule,
    CommonModule,
    ToastModule,
    UploadFileComponent,
    TableModule,
    DialogModule,
  ],
  templateUrl: './upload-business-license.component.html',
  styleUrl: './upload-business-license.component.scss'
})
export class UploadBusinessLicenseComponent implements OnInit {
  maxFileSize = Number(environment.BUISINESS_LICENSE_MAX_SIZE) * 1024 * 1024;
  selectedFile!: File | null;
  isFileUploadDisabled = false;
  isSizeLimitExceeded = false;
  isExtensionInvalid = false;
  updateFileRef!: UploadFileComponent;
  isUploadVisible = false;
  uploadHistory = new Array<any>();
  fileName!: string

  sort!: { prop: string, dir: 'asc' | 'desc' }

  constructor(private BLService: BusinessLicenceService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.getHistoryRecords();
  }

  getHistoryRecords(): void {
    this.loaderService.loadingStart();
    this.BLService.getUploadHistory().subscribe({
      next: (records) => {
        this.uploadHistory = records;
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    })
  }

  onUploadFile(): void {
    this.onUploadClose();
    this.BLService.uploadFile(this.selectedFile).subscribe({
      next: () => {
        //     TODO: toast and refresh page
      }
    })
  }

  onFileSelected(files: any): void {
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

    this.uploadHistory = this.uploadHistory.sort((a, b) => {
      return this.sortHandler(a[prop], b[prop]) * (this.sort.dir === 'asc' ? 1 : -1);
    });
  }

  private sortHandler(a: string | number, b: string | number): number {
    if (a > b) return 1;
    if (a < b) return -1;
    return 0;
  }

  onClear(): void {
    this.selectedFile = null;
    this.isFileUploadDisabled = false;
  }
}
