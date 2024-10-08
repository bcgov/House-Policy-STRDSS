import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Platform } from '../../../common/models/platform';
import { OrganizationService } from '../../../common/services/organization.service';
import { PagingResponse, PagingResponsePageInfo } from '../../../common/models/paging-response';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { TooltipModule } from 'primeng/tooltip';
import { Router, RouterModule } from '@angular/router';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

@Component({
  selector: 'app-platform-management',
  standalone: true,
  imports: [
    TableModule,
    CommonModule,
    ButtonModule,
    TableModule,
    PaginatorModule,
    TooltipModule,
    RouterModule,
  ],
  templateUrl: './platform-management.component.html',
  styleUrl: './platform-management.component.scss'
})
export class PlatformManagementComponent implements OnInit {
  @ViewChild('paginator') paginator!: Paginator;
  platforms = new Array<Platform>();

  sort!: { prop: string; dir: 'asc' | 'desc' };
  sortSub!: { prop: string; dir: 'asc' | 'desc' };
  currentPage!: PagingResponsePageInfo;

  constructor(
    private orgService: OrganizationService,
    private cd: ChangeDetectorRef,
    private router: Router,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.init();
  }

  onAddNewPlatform(): void {
    this.router.navigateByUrl('/add-new-platform');
  }

  editPlatform(id: number): void {
    this.router.navigateByUrl(`/platform/${id}`);
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

    this.platforms.forEach(p => {
      if (this.sortSub.dir === 'asc')
        p.subsidiaries = p.subsidiaries.sort((a: any, b: any) => this.sortHandler(a[property], b[property]));
      else
        p.subsidiaries = p.subsidiaries.sort((a: any, b: any) => this.sortHandler(b[property], a[property]));
    });
  }

  onPageChange(value: any): void {
    this.currentPage.pageSize = value.rows;
    this.currentPage.pageNumber = value.page + 1;

    this.getPlatforms(this.currentPage.pageNumber);
  }

  private getPlatforms(selectedPageNumber: number = 1): void {
    this.loaderService.loadingStart();
    this.orgService.getPlatforms(
      this.currentPage?.pageSize || 10,
      selectedPageNumber ?? (this.currentPage?.pageNumber || 0),
      this.sort?.prop || '',
      this.sort?.dir as any || 'asc',
    ).subscribe({
      next: (result: PagingResponse<Platform>) => {
        this.currentPage = result.pageInfo;

        this.platforms = result.sourceList.map((x, index) =>
          ({ ...x, id: `${x.organizationId}-${index + 1}` })
        );
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
