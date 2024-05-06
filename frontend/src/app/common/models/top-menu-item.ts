export interface TopMenuItem {
    title: string;
    description?: string;
    folderName?: string;
    icon?: string;
    route: string;
    accessPermission: string;
    isItOrgTypeBased?: boolean;
    orgType?: string;
    disabled?: boolean;
    hidden?: boolean;
    buttonId: string;
}