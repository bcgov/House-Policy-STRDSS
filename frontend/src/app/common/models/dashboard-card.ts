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
}