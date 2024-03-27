import { Routes } from '@angular/router';
import { ComplianceNoticeComponent } from './features/components/compliance-notice/compliance-notice.component';
import { DashboardComponent } from './features/components/dashboard/dashboard.component';
import { DelistingRequestComponent } from './features/components/delisting-request/delisting-request.component';
import { AccessRequestComponent } from './features/components/access-request/access-request.component';
import { AccessRequestListComponent } from './features/components/access-request-list/access-request-list.component';

export const routes: Routes = [
    {
        path: '',
        component: DashboardComponent,
    },
    {
        path: 'compliance-notice',
        component: ComplianceNoticeComponent,
    },
    {
        path: 'delisting-request',
        component: DelistingRequestComponent,
    },
    {
        path: 'access-request',
        component: AccessRequestComponent,
    },
    {
        path: 'access-request-list',
        component: AccessRequestListComponent,
    },

];
