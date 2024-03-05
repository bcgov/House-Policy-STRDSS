import { Component, Input, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { AuditApiService } from '../../services/api/audit-api.service';
import { HistoryRow } from '../../models/history-row.model';

@Component({
  selector: 'app-history-table',
  standalone: true,
  imports: [TableModule],
  templateUrl: './history-table.component.html',
  styleUrl: './history-table.component.scss'
})
export class HistoryTableComponent implements OnInit {
  @Input() applicationId!: number;
  historyRows = new Array<HistoryRow>();

  constructor(private auditService: AuditApiService) {
  }
  ngOnInit(): void {
    this.auditService.getStrApplicationsHistory(this.applicationId).subscribe((rows) => {
      this.historyRows = rows;
    })
  }
}
