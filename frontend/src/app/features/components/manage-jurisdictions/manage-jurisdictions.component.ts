import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { OrganizationService } from '../../../common/services/organization.service';
import { Router, RouterModule } from '@angular/router';
import { TooltipModule } from 'primeng/tooltip';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { LocalGovernment } from '../../../common/models/jurisdiction';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

@Component({
  selector: 'app-manage-jurisdictions',
  standalone: true,
  imports: [
    TableModule,
    CommonModule,
    ButtonModule,
    PaginatorModule,
    TooltipModule,
    RouterModule,
  ],
  templateUrl: './manage-jurisdictions.component.html',
  styleUrl: './manage-jurisdictions.component.scss'
})
export class ManageJurisdictionsComponent implements OnInit {
  @ViewChild('paginator') paginator!: Paginator;
  jurisdictions = new Array<LocalGovernment>();
  sort!: { prop: string; dir: 'asc' | 'desc' };
  sortSub!: { prop: string; dir: 'asc' | 'desc' };
  currentPage!: PagingResponsePageInfo;

  constructor(
    private organizationsService: OrganizationService,
    private cd: ChangeDetectorRef,
    private router: Router,
    private loaderService: GlobalLoaderService,
  ) {

  }

  ngOnInit(): void {
    this.init();
  }

  onSort(property: string): void {
    if (this.sort) {
      if (this.sort.prop === property) {
        this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
      } else {
        this.sort.prop = property;
        this.sort.dir = 'asc';
      }
    } else {
      this.sort = { prop: property, dir: 'asc' };
    }

    this.paginator.changePage(0);
  }

  onSortSub(property: string): void {
    if (this.sortSub) {
      if (this.sortSub.prop === property) {
        this.sortSub.dir = this.sortSub.dir === 'asc' ? 'desc' : 'asc';
      } else {
        this.sortSub.prop = property;
        this.sortSub.dir = 'asc';
      }
    } else {
      this.sortSub = { prop: property, dir: 'asc' };
    }

    this.jurisdictions.forEach(p => {
      if (this.sortSub.dir === 'asc')
        p.jurisdictions = p.jurisdictions.sort((a: any, b: any) => this.sortHandler(a[property], b[property]));
      else
        p.jurisdictions = p.jurisdictions.sort((a: any, b: any) => this.sortHandler(b[property], a[property]));
    });
  }

  onPageChange(value: any): void {
    this.currentPage.pageSize = value.rows;
    this.currentPage.pageNumber = value.page + 1;

    this.getPlatforms(this.currentPage.pageNumber);
  }

  private getPlatforms(selectedPageNumber: number = 1): void {
    this.loaderService.loadingStart();
    this.organizationsService.getJurisdictions(
      this.currentPage?.pageSize || 10,
      selectedPageNumber ?? (this.currentPage?.pageNumber || 0),
      this.sort?.prop || '',
      this.sort?.dir as any || 'asc',
    ).subscribe({
      next: (result: PagingResponse<LocalGovernment>) => {
        this.currentPage = result.pageInfo;
        this.jurisdictions = result.sourceList;

        console.info('First jurisdiction', this.jurisdictions[0]);

        this.cd.detectChanges();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      },
    });
  }

  private sortHandler(a: string, b: string): number {
    if (a > b) return 1;
    if (a < b) return -1;
    return 0;
  }

  private init(): void {
    this.getPlatforms();
  }
}
