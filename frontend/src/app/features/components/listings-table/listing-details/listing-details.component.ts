import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ListingTableRow } from '../../../../common/models/listing-table-row';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-listing-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    PanelModule,
  ],
  templateUrl: './listing-details.component.html',
  styleUrl: './listing-details.component.scss'
})
export class ListingDetailsComponent implements OnInit {
  @Input() listing!: ListingTableRow;
  @Output() closeEvent = new EventEmitter<'back' | 'close'>()

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  constructor() {

  }

  onClose(): void {
    this.closeEvent.emit('close');
  }

  onBack(): void {
    this.closeEvent.emit('back');
  }
  showLegend(): void {

  }
}
