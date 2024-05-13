import { Component } from '@angular/core';
import { ListingUploadHistoryTableComponent } from '../../../common/listing-upload-history-table/listing-upload-history-table.component';

@Component({
  selector: 'app-listing-upload-history',
  standalone: true,
  imports: [ListingUploadHistoryTableComponent],
  templateUrl: './listing-upload-history.component.html',
  styleUrl: './listing-upload-history.component.scss'
})
export class ListingUploadHistoryComponent {

}
