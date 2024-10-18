import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { TagModule } from 'primeng/tag';
import { OrganizationService } from '../../../../common/services/organization.service';
import { GlobalLoaderService } from '../../../../common/services/global-loader.service';
import { Platform } from '../../../../common/models/platform';

@Component({
  selector: 'app-view-platform',
  standalone: true,
  imports: [
    PanelModule,
    TagModule,
    ButtonModule,
    RouterModule,
    CommonModule,
  ],
  templateUrl: './view-platform.component.html',
  styleUrl: './view-platform.component.scss'
})
export class ViewPlatformComponent implements OnInit {
  id!: number;
  platform!: Platform;

  constructor(
    private orgService: OrganizationService,
    private cd: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.init();
  }

  onAddSubPlatform(): void {
    this.router.navigateByUrl(`/add-sub-platform/${this.id}`);
  }

  onEditPlatform(id: number): void {
    this.router.navigateByUrl(`/edit-platform/${id}`);
  }
  onEditSubPlatform(id: number): void {
    this.router.navigateByUrl(`/edit-sub-platform/${id}`);
  }

  private init(): void {
    this.loaderService.loadingStart
    this.orgService.getPlatform(this.id).subscribe({
      next: (x) => {
        this.platform = x as Platform;
        this.cd.detectChanges();
      },
      complete: () => {
        this.loaderService.loadingEnd();
      }
    });
  }
}
