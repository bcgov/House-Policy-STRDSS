import { Routes } from '@angular/router';
import { ComplianceNoticeComponent } from './features/components/compliance-notice/compliance-notice.component';
import { DashboardComponent } from './features/components/dashboard/dashboard.component';

export const routes: Routes = [
    {
        path: '',
        component: DashboardComponent,
    },
    {
        path: 'compliance-notice',
        component: ComplianceNoticeComponent
    }
];
