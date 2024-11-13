import { Routes } from '@angular/router';
import { ComplianceNoticeComponent } from './features/components/compliance-notice/compliance-notice.component';
import { DashboardComponent } from './features/components/dashboard/dashboard.component';
import { DelistingRequestComponent } from './features/components/delisting-request/delisting-request.component';
import { AccessRequestComponent } from './features/components/access-request/access-request.component';
import { UserManagementComponent } from './features/components/user-management/user-management.component';
import { UnauthorizedComponent } from './common/components/unauthorized/unauthorized.component';
import { PageNotFoundComponent } from './common/components/page-not-found/page-not-found.component';
import { approvedUserGuard } from './common/guards/approved-user.guard';
import { activeUserGuard } from './common/guards/active-user.guard';
import { accessRequestTokenGuard } from './common/guards/access-request-token.guard';
import { jurisdiction_read, jurisdiction_write, licence_file_upload, listing_file_upload, listing_read, platform_read, platform_write, role_read, role_write, takedown_action, upload_history_read, user_read, user_write } from './common/consts/permissions.const';
import { hasPermissionsGuard } from './common/guards/has-permissions.guard';
import { TermsAndConditionsComponent } from './common/components/terms-and-conditions/terms-and-conditions.component';
import { areTermsAceptedGuard } from './common/guards/are-terms-acepted.guard';
import { UploadListingsComponent } from './features/components/upload-listings/upload-listings.component';
import { ListingUploadHistoryComponent } from './features/components/listing-upload-history/listing-upload-history.component';
import { ListingsTableComponent } from './features/components/listings-table/listings-table.component';
import { ListingDetailsComponent } from './features/components/listings-table/listing-details/listing-details.component';
import { BulkTakedownRequestComponent } from './features/components/bulk-takedown-request/bulk-takedown-request.component';
import { BulkComplianceNoticeComponent } from './features/components/bulk-compliance-notice/bulk-compliance-notice.component';
import { RolesListComponent } from './features/components/roles-list/roles-list.component';
import { RoleDetailsComponent } from './features/components/roles-list/role-details/role-details.component';
import { UserDetailsComponent } from './features/components/user-management/user-details/user-details.component';
import { ExportListingsComponent } from './features/components/export-listings/export-listings.component';
import { UploadBusinessLicenseComponent } from './features/components/upload-business-license/upload-business-license.component';
import { AggregatedListingsTableComponent } from './features/components/listings-table/aggregated-listings-table/aggregated-listings-table.component';
import { AddApsUserComponent } from './features/components/user-management/add-aps-user/add-aps-user.component';
import { PlatformManagementComponent } from './features/components/platform-management/platform-management.component';
import { AddNewPlatformComponent } from './features/components/platform-management/add-new-platform/add-new-platform.component';
import { AddSubPlatformComponent } from './features/components/platform-management/add-sub-platform/add-sub-platform.component';
import { EditPlatformComponent } from './features/components/platform-management/edit-platform/edit-platform.component';
import { ViewPlatformComponent } from './features/components/platform-management/view-platform/view-platform.component';
import { EditSubPlatformComponent } from './features/components/platform-management/edit-sub-platform/edit-sub-platform.component';
import { ManageJurisdictionsComponent } from './features/components/manage-jurisdictions/manage-jurisdictions.component';
import { UpdateJurisdictionInformationComponent } from './features/components/manage-jurisdictions/update-jurisdiction-information/update-jurisdiction-information.component';
import { UpdateLocalGovernmentInformationComponent } from './features/components/manage-jurisdictions/update-local-gvernment-information/update-local-gvernment-information.component';

export const routes: Routes = [
    {
        path: '',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard],
        component: DashboardComponent,
    },
    {
        path: 'upload-listing-data',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        component: UploadListingsComponent,
        data: { permissions: [listing_file_upload] }
    },
    {
        path: 'upload-business-licence-data',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        component: UploadBusinessLicenseComponent,
        data: { permissions: [licence_file_upload] }
    },
    {
        path: 'upload-listing-history',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        component: ListingUploadHistoryComponent,
        data: { permissions: [upload_history_read] }
    },
    {
        path: 'terms-and-conditions',
        canActivate: [approvedUserGuard, activeUserGuard],
        component: TermsAndConditionsComponent,
    },
    {
        path: 'compliance-notice',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: ComplianceNoticeComponent,
        data: { permissions: [takedown_action] }
    },
    {
        path: 'takedown-request',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: DelistingRequestComponent,
        data: { permissions: [takedown_action] }
    },
    {
        path: 'bulk-takedown-request',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: BulkTakedownRequestComponent,
        data: { permissions: [takedown_action] }
    },
    {
        path: 'bulk-compliance-notice',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: BulkComplianceNoticeComponent,
        data: { permissions: [takedown_action] }
    },
    {
        path: 'listings',
        component: ListingsTableComponent,
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'aggregated-listings',
        component: AggregatedListingsTableComponent,
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'listing/:id', component: ListingDetailsComponent,
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'access-request',
        canActivate: [accessRequestTokenGuard],
        component: AccessRequestComponent,
    },
    {
        path: 'user-management',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: UserManagementComponent,
        data: { permissions: [user_write] }
    },
    {
        path: 'user-management',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: UserManagementComponent,
        data: { permissions: [user_write] }
    },
    {
        path: 'manage-jurisdictions',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: ManageJurisdictionsComponent,
        data: { permissions: [jurisdiction_read] }
    },
    {
        path: 'update-local-government-information/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: UpdateLocalGovernmentInformationComponent,
        data: { permissions: [jurisdiction_write] }
    },
    {
        path: 'update-jurisdiction-information/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: UpdateJurisdictionInformationComponent,
        data: { permissions: [jurisdiction_write] }
    },
    {
        path: 'platform-management',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: PlatformManagementComponent,
        data: { permissions: [platform_read] }
    },
    {
        path: 'add-new-platform',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: AddNewPlatformComponent,
        data: { permissions: [platform_write] }
    },
    {
        path: 'add-sub-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: AddSubPlatformComponent,
        data: { permissions: [platform_write] }
    },
    {
        path: 'edit-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: EditPlatformComponent,
        data: { permissions: [platform_write] }
    },
    {
        path: 'edit-sub-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: EditSubPlatformComponent,
        data: { permissions: [platform_write] }
    },
    {
        path: 'platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: ViewPlatformComponent,
        data: { permissions: [platform_read] }
    },
    {
        path: 'user/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: UserDetailsComponent,
        data: { permissions: [user_read] }
    },
    {
        path: 'add-aps-user',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: AddApsUserComponent,
        data: { permissions: [user_read, user_write] }
    },
    {
        path: 'roles',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: RolesListComponent,
        data: { permissions: [role_read] }
    },
    {
        path: 'role/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: RoleDetailsComponent,
        data: { permissions: [role_write] }
    },
    {
        path: 'role',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: RoleDetailsComponent,
        data: { permissions: [role_write] }
    },
    {
        path: 'export-listings',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        component: ExportListingsComponent,
        data: { permissions: [listing_read] }
    },
    {
        path: '401',
        component: UnauthorizedComponent,
    },
    {
        path: '404',
        component: PageNotFoundComponent
    },
    {
        path: '**',
        redirectTo: '/404'
    }
];
