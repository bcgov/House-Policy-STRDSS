import { Component, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import {
  PutApplication,
  StrApplication,
} from '../../../common/models/application.model';
import { StrApplicationsApiService } from '../../../common/services/api/str-applications-api.service';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { StoreService } from '../../../common/services/store.service';
import { CommonCode } from '../../../common/models/common-code.model';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { CommonModule } from '@angular/common';
import { MapComponent } from '../../../common/components/map/map.component';
import { MapManipulationService } from '../../../common/services/map-manipulation.service';
import { LatLonCoordinate } from '../../../common/models/lat-lon-coordinates.model';
import { AuthenticatedUser } from '../../../common/models/user';
import { SidebarModule } from 'primeng/sidebar';
import { HistoryTableComponent } from '../../../common/components/history-table/history-table.component';
import { Coordinate } from 'ol/coordinate';
import { LineString } from 'ol/geom';
import { fromLonLat } from 'ol/proj';
@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [
    MapComponent,
    CardModule,
    CommonModule,
    TableModule,
    InputTextModule,
    InputNumberModule,
    ButtonModule,
    DialogModule,
    DropdownModule,
    CheckboxModule,
    ToastModule,
    FormsModule,
    ReactiveFormsModule,
    SidebarModule,
    HistoryTableComponent,
  ],
  templateUrl: './applications.component.html',
  styleUrl: './applications.component.scss',
})
export class ApplicationsComponent implements OnInit {
  applications = new Array<StrApplication>();
  allApplications = new Array<StrApplication>();
  isAddDialogVisible = false;
  strAffiliates!: Array<CommonCode>;
  complianceStatuses!: Array<CommonCode>;
  zoningTypes!: Array<CommonCode>;
  isAdmin = false;
  userData!: AuthenticatedUser;
  showLogs = false;
  isCreate = true;
  currentApplication!: StrApplication | null;
  propertyForm!: FormGroup;

  constructor(
    private strAppService: StrApplicationsApiService,
    private fb: FormBuilder,
    private storeService: StoreService,
    private messageService: MessageService,
    private mapManipulationService: MapManipulationService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.isUserAdmin();
    this.getApplications();

    this.mapManipulationService.circleFilter$.subscribe((circle) => {
      this.applications = this.filterApplicationsByCircle(
        circle.center,
        circle.radius
      );
    });
  }
  private filterApplicationsByCircle(
    center: Coordinate,
    radius: number
  ): StrApplication[] {
    return this.allApplications.filter((app) => {
      const appCoordinate: Coordinate = fromLonLat(
        [app.latitude, app.longitude],
        'EPSG:3857'
      );
      const distance = this.getDistance(center, appCoordinate);
      return distance <= radius;
    });
  }

  getDistance = (latlng1: Coordinate, latlng2: Coordinate): number => {
    const line = new LineString([latlng1, latlng2]);
    return Math.round(line.getLength() * 100) / 100;
  };

  onAddApplication(): void {
    this.isCreate = true;
    this.initializeCreateForm();
    this.openAddApplicationDialog();
  }

  onSubmitApplication(): void {
    const formData: PutApplication = this.propertyForm.value;

    if (this.isCreate) {
      this.strAppService.postStrApplication(formData).subscribe({
        next: (_data) => {
          this.getApplications();
          this.closeApplicationDialog();
        },
        error: (e) => {
          if (e && e.status === 422 && e.error) {
            Object.keys(e.error.errors).forEach((errorKey: string) => {
              this.messageService.add({
                severity: 'error',
                summary: `${errorKey} validation error`,
                detail: (e.error.errors[errorKey] as Array<string>).toString(),
              });
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'Unkonw error',
              detail: e.status,
            });
          }
        },
      });
    } else {
      formData.id = this.currentApplication?.id || -1;
      if (!formData.complianceStatusId) {
        formData.complianceStatusId =
          this.currentApplication?.complianceStatusId || -1;
      }
      formData.applicantId = this.currentApplication?.applicantId || -1;

      this.strAppService.putStrApplication(formData).subscribe({
        next: (data) => {
          this.getApplications();
          this.closeApplicationDialog();
        },
        error: (e) => {
          if (e && e.error) {
            Object.keys(e.error.errors).forEach((errorKey: string) => {
              this.messageService.add({
                severity: 'error',
                summary: `${errorKey} validation error`,
                detail: (e.error.errors[errorKey] as Array<string>).toString(),
              });
            });
          }
        },
      });
    }
  }

  onApplicationAddressClicked(application: StrApplication): void {
    const coordinate: LatLonCoordinate = {
      lat: application.latitude,
      lon: application.longitude,
    };

    if (application.latitude && application.longitude) {
      this.mapManipulationService.coordinateViewRequested.next(coordinate);
    } else {
      this.mapManipulationService.coordinateViewRequested.next({
        lat: -123.36116595637016,
        lon: 48.42702928270913,
      });
    }
  }

  closeApplicationDialog(): void {
    this.propertyForm.reset();
    this.isAddDialogVisible = false;
    this.currentApplication = null;
  }

  initializeCreateForm(): void {
    this.zoningTypes = this.storeService.zoneTypes;
    this.strAffiliates = this.storeService.strAffiliates;
    this.complianceStatuses = this.storeService.complianceStatuses;

    this.propertyForm = this.fb.group({
      streetAddress: ['', Validators.required],
      city: ['', Validators.required],
      province: ['', Validators.required],
      postalCode: ['', Validators.required],
      zoningTypeId: ['', Validators.required],
      squareFootage: [null, Validators.required],
      strAffiliateId: ['', Validators.required],
      lastName: [''],
      isOwnerPrimaryResidence: [false],
    });
  }

  initializeEditForm(application: StrApplication): void {
    this.zoningTypes = this.storeService.zoneTypes;
    this.strAffiliates = this.storeService.strAffiliates;
    this.complianceStatuses = this.storeService.complianceStatuses;

    this.propertyForm = this.fb.group({
      streetAddress: [application.streetAddress, Validators.required],
      city: [application.city, Validators.required],
      province: [application.province, Validators.required],
      postalCode: [application.postalCode, Validators.required],
      zoningTypeId: [application.zoningTypeId, Validators.required],
      squareFootage: [application.squareFootage, Validators.required],
      strAffiliateId: [application.strAffiliateId, Validators.required],
      complianceStatusId: [
        { value: application.complianceStatusId, disabled: !this.isAdmin },
        Validators.required,
      ],
      isOwnerPrimaryResidence: [application.isOwnerPrimaryResidence],
    });
  }

  openExistingApplication(application: StrApplication): void {
    this.currentApplication = application;
    this.isCreate = false;
    this.initializeEditForm(application);
    this.openAddApplicationDialog();
  }

  openLogsDrawer(): void {
    this.showLogs = true;
  }

  private isUserAdmin(): boolean {
    const userdataRaw = localStorage.getItem('user_data');

    if (userdataRaw) {
      this.userData = JSON.parse(userdataRaw);

      return this.userData.role === 'Admin';
    }

    return false;
  }

  private openAddApplicationDialog(): void {
    this.isAddDialogVisible = true;
  }

  private getApplications(): void {
    this.strAppService.getStrApplications().subscribe((data) => {
      this.applications = data;
      this.allApplications = data;
      this.loadApplicationPins();
    });
  }

  private loadApplicationPins(): void {
    this.mapManipulationService.triggerCleanUpVectorLayers();
    this.applications.map((app) => {
      this.mapManipulationService.applicationPin.next(app);
    });
  }
}
