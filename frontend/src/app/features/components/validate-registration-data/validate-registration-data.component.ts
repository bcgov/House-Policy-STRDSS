import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { CommonModule } from '@angular/common';
import { DelistingService } from '../../../common/services/delisting.service';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { UserDataService } from '../../../common/services/user-data.service';
import { forkJoin } from 'rxjs';
import { User } from '../../../common/models/user';
import { environment } from '../../../../environments/environment';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { UploadFileComponent } from '../../../common/components/upload-file/upload-file.component';
import { RegistrationService } from '../../../common/services/registration.service';

@Component({
  selector: 'app-validate-registration-data',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DropdownModule,
    ButtonModule,
    CommonModule,
    ToastModule,
    UploadFileComponent,
  ],
  templateUrl: './validate-registration-data.component.html',
  styleUrl: './validate-registration-data.component.scss'
})
export class ValidateRegistrationDataComponent implements OnInit {
  private readonly LISTING_TYPE = 0;
  private readonly TAKEDOWN_TYPE = 1;
  platformOptions = new Array<DropdownOption>();  
  maxFileSize = Number(environment.RENTAL_LISTING_REPORT_MAX_SIZE) * 1024 * 1024;
  uploadedFile: any;
  uploadElem!: any;
  currentUser!: User;
  isUploadStarted = false;
  isFileUploadDisabled = false;

  isSizeLimitExceeded = false;
  isExtensionInvalid = false;

  updateFileRef!: UploadFileComponent

  myForm = this.fb.group({
    platformId: [0, Validators.required],
    file: ['', Validators.required],
  });

  public get platformIdControl(): AbstractControl {
    return this.myForm.controls['platformId'];
  }

  public get fileControl(): AbstractControl {
    return this.myForm.controls['file'];
  }

  constructor(
    private fb: FormBuilder,
    private delistingService: DelistingService,
    private registrationService: RegistrationService,
    private messageService: MessageService,
    private userDataService: UserDataService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    const getCurrentUser = this.userDataService.getCurrentUser();
    const getPlatforms = this.delistingService.getPlatforms();

    forkJoin([getCurrentUser, getPlatforms]).subscribe({
      next: ([currentUser, platforms]) => {
        this.currentUser = currentUser;
        if (currentUser.organizationType !== "Platform") {
          this.platformOptions = platforms;
        } else {
          this.myForm.controls['platformId'].setValue(currentUser.organizationId);
        }
        this.cd.detectChanges();
      },
    });
  }

  onFileSelected(files: any, componentRef: UploadFileComponent): void {
    this.updateFileRef = componentRef;

    if (!!files) {
      this.myForm.controls['file'].setValue(files[0]);
      this.myForm.controls['file'].markAsDirty();
      this.uploadedFile = files[0];
      this.isFileUploadDisabled = !!files.length;
    }

    this.cd.detectChanges();
  }

  onClear(): void {
    this.myForm.controls['file'].setValue(null);
    this.uploadedFile = null;
    this.isFileUploadDisabled = false;
    this.cd.detectChanges();
  }

  onUpload(): void {
    this.isUploadStarted = true;
    const formResult = this.myForm.value;
    this.loaderService.loadingStart('Uploading');

    const request = this.registrationService.uploadRegistrations(formResult.platformId || 0, this.uploadedFile);

    request.subscribe({
      next: (_res) => {
        this.myForm.reset();
        this.onClear();

        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File has been uploaded successfully' });
      },
      complete: () => {
        this.loaderService.loadingEnd();
        setTimeout(() => {
          this.isUploadStarted = false;
        }, 300);
      }
    });
  }
}
