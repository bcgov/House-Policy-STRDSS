import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagesModule } from 'primeng/messages';
import { Message } from 'primeng/api';

@Component({
  selector: 'app-export-listings',
  standalone: true,
  imports: [
    RadioButtonModule,
    ButtonModule,
    CommonModule,
    FormsModule,
    MessagesModule,
  ],
  templateUrl: './export-listings.component.html',
  styleUrl: './export-listings.component.scss'
})
export class ExportListingsComponent implements OnInit {
  dateLastUpdated = '';
  jurisdictions = new Array<DropdownOption>();
  selectedJurisdiction!: any;

  constructor(
    private cd: ChangeDetectorRef,
    private errorService: ErrorHandlingService,
    private loaderService: GlobalLoaderService,
  ) {

  }

  ngOnInit(): void {

    this.jurisdictions.push(...[
      {
        label: 'Test LG',
        value: 'test_lg'
      },
      {
        label: 'Test LG 2',
        value: 'test_lg_2'
      },
      {
        label: 'Test LG 3',
        value: 'test_lg_3'
      },
      {
        label: 'Test LG 4',
        value: 'test_lg_4'
      },
      {
        label: 'Test LG 5',
        value: 'test_lg_5'
      },
      {
        label: 'Test LG 6',
        value: 'test_lg_6'
      },
      {
        label: 'Test LG 7',
        value: 'test_lg_7'
      },
      {
        label: 'Test LG 8',
        value: 'test_lg_8'
      },
      {
        label: 'Test LG 9',
        value: 'test_lg_9'
      },
      {
        label: 'Test LG 10',
        value: 'test_lg_10'
      },
      {
        label: 'Test LG 11',
        value: 'test_lg_11'
      },
      {
        label: 'Test LG 12',
        value: 'test_lg_12'
      },
      {
        label: 'Test LG 13',
        value: 'test_lg_13'
      },
      {
        label: 'Test LG 14',
        value: 'test_lg_14'
      },
      {
        label: 'Test LG 15',
        value: 'test_lg_15'
      },
      {
        label: 'Test LG 16',
        value: 'test_lg_16'
      },
      {
        label: 'Test LG 17',
        value: 'test_lg_17'
      },
      {
        label: 'Test LG 18',
        value: 'test_lg_18'
      },
      {
        label: 'Test LG 19',
        value: 'test_lg_19'
      },
      {
        label: 'Test LG 20',
        value: 'test_lg_20'
      },
    ])
  }

  onDownload(): void {

  }
}
