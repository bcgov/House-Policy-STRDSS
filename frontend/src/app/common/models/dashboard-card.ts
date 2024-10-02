export interface DashboardCard {
    title: string;
    description: string;
    buttonText: string;
    buttonIcon: string;
    route: string;
    accessPermission: string;
    isItOrgTypeBased?: boolean;
    isButtonDisabled?: boolean;
    orgType?: string;
    hidden?: boolean;
    boxId?: string;
    buttonId?: string;
    isComingSoon?: boolean;
    section: 'main' | 'admin' | 'forms' | 'info';
}

export interface DashboardCardSections {
    main: DashboardCard[];
    admin: DashboardCard[];
    forms: DashboardCard[];
    info: DashboardCard[];
}