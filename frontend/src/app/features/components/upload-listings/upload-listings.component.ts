import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { validateExtension, validateFileSize } from '../../../common/consts/validators.const';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { CommonModule } from '@angular/common';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { DelistingService } from '../../../common/services/delisting.service';
import { ToastModule } from 'primeng/toast';
import { ListingDataService } from '../../../common/services/listing-data.service';
import { YearMonthGenService } from '../../../common/services/year-month-gen.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-upload-listings',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DropdownModule,
    ButtonModule,
    CommonModule,
    FileUploadModule,
    ToastModule,
  ],
  providers: [FileReader],
  templateUrl: './upload-listings.component.html',
  styleUrl: './upload-listings.component.scss'
})
export class UploadListingsComponent implements OnInit {
  platformOptions = new Array<DropdownOption>();
  monthsOptions = new Array<DropdownOption>();
  maxFileSize = 4000000;
  uploadedFile: any;
  uploadElem!: FileUpload;

  myForm = this.fb.group({
    platformId: [0, Validators.required],
    month: ['', Validators.required],
    file: ['', [Validators.required,
    validateExtension('text/csv'),
    validateFileSize(this.maxFileSize)]]
  });

  public get platformIdControl(): AbstractControl {
    return this.myForm.controls['platformId'];
  }

  public get monthControl(): AbstractControl {
    return this.myForm.controls['month'];
  }

  public get fileControl(): AbstractControl {
    return this.myForm.controls['file'];
  }

  constructor(
    private fb: FormBuilder,
    private delistingService: DelistingService,
    private listingDataService: ListingDataService,
    private yearMonthGenService: YearMonthGenService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    this.monthsOptions = this.yearMonthGenService.getPreviousMonths(10);
    this.delistingService.getPlatforms().subscribe((platformOptions) => this.platformOptions = platformOptions);
  }

  onFileSelected(event: any, uploadElem: FileUpload): void {
    if (!!event.currentFiles) {
      this.myForm.controls['file'].setValue(event.currentFiles[0]);
      this.myForm.controls['file'].markAsDirty();
      this.uploadElem = uploadElem;
      this.uploadedFile = event.currentFiles[0];
      this.uploadElem.disabled = true;
    }
  }
  onClear(): void {
    this.uploadElem.clear();
    this.myForm.controls['file'].setValue(null);
    this.uploadedFile = null;
    this.uploadElem.disabled = false;
  }

  onUpload(): void {
    const formResult = this.myForm.value;

    this.listingDataService.uploadData(formResult.month || '', formResult.platformId || 0, this.uploadedFile)
      .subscribe({
        next: (_res) => {
          this.myForm.reset();
          this.onClear();

          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File has been uploaded successfully' });
        },
      });
  }
}
