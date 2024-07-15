import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ErrorHandlingService } from '../../services/error-handling.service';

@Component({
  selector: 'app-upload-file',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
  ],
  templateUrl: './upload-file.component.html',
  styleUrl: './upload-file.component.scss'
})
export class UploadFileComponent {
  @Input() sizeLimit: number = 100000;
  @Input() extension: string = '.csv';
  @Input() multiple: boolean = false;
  @Input() disabled: boolean = false;

  @Output() filesSelected = new EventEmitter<File[]>();

  constructor(private messageService: ErrorHandlingService) { }

  openFileDialog() {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.multiple = this.multiple;
    fileInput.accept = this.extension;

    fileInput.addEventListener('change', (event: any) => {
      const files = event?.target?.files || [];

      if (files && files.length > 0) {
        const validFiles = [];

        for (const file of files) {
          if (file.size <= this.sizeLimit) {
            if (file.name.toLowerCase().endsWith(this.extension.toLowerCase())) {
              validFiles.push(file);
            } else {
              this.messageService.showError('Wrong file type');
            }
          } else {
            this.messageService.showError(`File size should not be bigger
                than ${this.sizeLimit} bytes.`);
          }
        }

        this.filesSelected.emit(validFiles);
      }
    });

    fileInput.click();
  }
}
