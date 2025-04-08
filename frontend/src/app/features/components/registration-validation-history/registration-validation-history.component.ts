import { ChangeDetectorRef, Component } from '@angular/core';
import { DropdownOption } from '../../../common/models/dropdown-option';
import { User } from '../../../common/models/user';
import { PagingResponsePageInfo } from '../../../common/models/paging-response';
import { DelistingService } from '../../../common/services/delisting.service';
import { UserDataService } from '../../../common/services/user-data.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { forkJoin } from 'rxjs';
import { RegistrationService } from '../../../common/services/registration.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-registration-validation-history',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    PaginatorModule,
    DropdownModule,
    CommonModule,
    RouterModule,
    CardModule,
    ReactiveFormsModule,
  ],
  templateUrl: './registration-validation-history.component.html',
  styleUrl: './registration-validation-history.component.scss'
})
export class RegistrationValidationHistoryComponent {
  registrationValidationHistory = new Array<any>();
  platformOptions = new Array<DropdownOption>();
  selectedPlatformId = 0;
  sort!: { prop: string, dir: 'asc' | 'desc' }
  currentUser!: User;
  currentPage!: PagingResponsePageInfo;

  constructor(
    private registrationService: RegistrationService,
    private delistingService: DelistingService,
    private userDataService: UserDataService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    const getCurrentUser = this.userDataService.getCurrentUser()
    const getPlatforms = this.delistingService.getPlatforms();

    forkJoin([getCurrentUser, getPlatforms]).subscribe({
      next: ([currentUser, platforms]) => {
        this.currentUser = currentUser;
        if (currentUser.organizationType !== "Platform") {
          const options: Array<DropdownOption> = [{ label: 'All', value: 0 }, ...platforms]
          this.platformOptions = options;
        }
      },
      complete: () => {
        this.cd.detectChanges();
      }
    });

    this.delistingService.getPlatforms().subscribe((platformOptions) => {
      const options: Array<DropdownOption> = [{ label: 'All', value: 0 }, ...platformOptions]
      this.platformOptions = options;
    });

    this.getRegistrationValidationHistory(1);
  }

  onPlatformSelected(_value: number): void {
    this.getRegistrationValidationHistory(1);
  }

  onSort(property: string): void {
    if (this.sort) {
      if (this.sort.prop === property) {
        this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
      }
      else {
        this.sort.prop = property;
        this.sort.dir = 'asc';
      }
    }
    else {
      this.sort = { prop: property, dir: 'asc' };
    }

    this.getRegistrationValidationHistory(this.currentPage.pageNumber);
  }

  onPageChange(value: any): void {
    this.getRegistrationValidationHistory(value.page + 1);
  }

  private getRegistrationValidationHistory(selectedPageNumber: number = 1): void {
    this.loaderService.loadingStart();
    this.registrationService.getRegistrationValidationHistory(
      selectedPageNumber ?? (this.currentPage?.pageNumber || 0), 
      this.currentPage?.pageSize || 10,
      this.sort?.prop || '',
      this.sort?.dir || 'asc',
      this.selectedPlatformId).subscribe({
      next: (records) => {
        this.currentPage = records.pageInfo;
        this.registrationValidationHistory = records.sourceList;
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      }
    });
  }
}
