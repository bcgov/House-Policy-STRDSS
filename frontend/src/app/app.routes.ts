import { Routes } from '@angular/router';
import { UnauthorizedComponent } from './common/components/unauthorized/unauthorized.component';
import { PageNotFoundComponent } from './common/components/page-not-found/page-not-found.component';
import { approvedUserGuard } from './common/guards/approved-user.guard';
import { activeUserGuard } from './common/guards/active-user.guard';
import { accessRequestTokenGuard } from './common/guards/access-request-token.guard';
import { ceu_action, jurisdiction_read, jurisdiction_write, licence_file_upload, listing_file_upload, listing_read, platform_read, platform_write, role_read, role_write, takedown_action, upload_history_read, user_read, user_write, validate_reg_no } from './common/consts/permissions.const';
import { hasPermissionsGuard } from './common/guards/has-permissions.guard';
import { areTermsAceptedGuard } from './common/guards/are-terms-acepted.guard';

export const routes: Routes = [
    {
        path: '',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    },
    {
        path: 'upload-listing-data',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        loadComponent: () => import('./features/components/upload-listings/upload-listings.component').then(m => m.UploadListingsComponent),
        data: { permissions: [listing_file_upload] }
    },
    {
        path: 'upload-business-licence-data',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        loadComponent: () => import('./features/components/upload-business-license/upload-business-license.component').then(m => m.UploadBusinessLicenseComponent),
        data: { permissions: [licence_file_upload] }
    },
    {
        path: 'upload-listing-history',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        loadComponent: () => import('./features/components/listing-upload-history/listing-upload-history.component').then(m => m.ListingUploadHistoryComponent),
        data: { permissions: [upload_history_read] }
    },
    {
        path: 'terms-and-conditions',
        canActivate: [approvedUserGuard, activeUserGuard],
        loadComponent: () => import('./common/components/terms-and-conditions/terms-and-conditions.component').then(m => m.TermsAndConditionsComponent),
    },
    {
        path: 'compliance-notice',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/compliance-notice/compliance-notice.component').then(m => m.ComplianceNoticeComponent),
        data: { permissions: [takedown_action] }
    },
    {
        path: 'takedown-request',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/delisting-request/delisting-request.component').then(m => m.DelistingRequestComponent),
        data: { permissions: [takedown_action] }
    },
    {
        path: 'bulk-takedown-request',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/bulk-takedown-request/bulk-takedown-request.component').then(m => m.BulkTakedownRequestComponent),
        data: { permissions: [takedown_action] }
    },
    {
        path: 'bulk-compliance-notice',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/bulk-compliance-notice/bulk-compliance-notice.component').then(m => m.BulkComplianceNoticeComponent),
        data: { permissions: [takedown_action] }
    },
    {
        path: 'send-compliance-order',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/send-compliance-order/send-compliance-order.component').then(m => m.SendComplianceOrderComponent),
        data: { permissions: [ceu_action] }
    },
    {
        path: 'listings',
        loadComponent: () => import('./features/components/listings-table/listings-table.component').then(m => m.ListingsTableComponent),
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'aggregated-listings',
        loadComponent: () => import('./features/components/listings-table/aggregated-listings-table/aggregated-listings-table.component').then(m => m.AggregatedListingsTableComponent),
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'listing/:id',
        loadComponent: () => import('./features/components/listings-table/listing-details/listing-details.component').then(m => m.ListingDetailsComponent),
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        data: { permissions: [listing_read] },
    },
    {
        path: 'access-request',
        canActivate: [accessRequestTokenGuard],
        loadComponent: () => import('./features/components/access-request/access-request.component').then(m => m.AccessRequestComponent),
    },
    {
        path: 'user-management',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/user-management/user-management.component').then(m => m.UserManagementComponent),
        data: { permissions: [user_write] }
    },
    {
        path: 'manage-jurisdictions',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/manage-jurisdictions/manage-jurisdictions.component').then(m => m.ManageJurisdictionsComponent),
        data: { permissions: [jurisdiction_read] }
    },
    {
        path: 'update-local-government-information/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/manage-jurisdictions/update-local-gvernment-information/update-local-gvernment-information.component').then(m => m.UpdateLocalGovernmentInformationComponent),
        data: { permissions: [jurisdiction_write] }
    },
    {
        path: 'update-jurisdiction-information/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/manage-jurisdictions/update-jurisdiction-information/update-jurisdiction-information.component').then(m => m.UpdateJurisdictionInformationComponent),
        data: { permissions: [jurisdiction_write] }
    },
    {
        path: 'platform-management',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/platform-management.component').then(m => m.PlatformManagementComponent),
        data: { permissions: [platform_read] }
    },
    {
        path: 'add-new-platform',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/add-new-platform/add-new-platform.component').then(m => m.AddNewPlatformComponent),
        data: { permissions: [platform_write] }
    },
    {
        path: 'add-sub-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/add-sub-platform/add-sub-platform.component').then(m => m.AddSubPlatformComponent),
        data: { permissions: [platform_write] }
    },
    {
        path: 'edit-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/edit-platform/edit-platform.component').then(m => m.EditPlatformComponent),
        data: { permissions: [platform_write] }
    },
    {
        path: 'edit-sub-platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/edit-sub-platform/edit-sub-platform.component').then(m => m.EditSubPlatformComponent),
        data: { permissions: [platform_write] }
    },
    {
        path: 'platform/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/platform-management/view-platform/view-platform.component').then(m => m.ViewPlatformComponent),
        data: { permissions: [platform_read] }
    },
    {
        path: 'user/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/user-management/user-details/user-details.component').then(m => m.UserDetailsComponent),
        data: { permissions: [user_read] }
    },
    {
        path: 'add-aps-user',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/user-management/add-aps-user/add-aps-user.component').then(m => m.AddApsUserComponent),
        data: { permissions: [user_read, user_write] }
    },
    {
        path: 'roles',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/roles-list/roles-list.component').then(m => m.RolesListComponent),
        data: { permissions: [role_read] }
    },
    {
        path: 'role/:id',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/roles-list/role-details/role-details.component').then(m => m.RoleDetailsComponent),
        data: { permissions: [role_write] }
    },
    {
        path: 'role',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/roles-list/role-details/role-details.component').then(m => m.RoleDetailsComponent),
        data: { permissions: [role_write] }
    },
    {
        path: 'export-listings',
        canActivate: [approvedUserGuard, activeUserGuard, hasPermissionsGuard, areTermsAceptedGuard],
        loadComponent: () => import('./features/components/export-listings/export-listings.component').then(m => m.ExportListingsComponent),
        data: { permissions: [listing_read] }
    },
    {
        path: 'validate-registration-data',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        loadComponent: () => import('./features/components/validate-registration-data/validate-registration-data.component').then(m => m.ValidateRegistrationDataComponent),
        data: { permissions: [validate_reg_no] }
    },
    {
        path: 'registration-validation-history',
        canActivate: [approvedUserGuard, activeUserGuard, areTermsAceptedGuard, hasPermissionsGuard],
        loadComponent: () => import('./features/components/registration-validation-history/registration-validation-history.component').then(m => m.RegistrationValidationHistoryComponent),
        data: { permissions: [validate_reg_no] }
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
