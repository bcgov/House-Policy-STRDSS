export interface DropdownOption {
    label: string;
    value: any;
}

export interface DropdownOptionOrganization {
    label: string;
    value: any;
    localGovernmentType?: string;
}

export interface DropdownOptionSelectable extends DropdownOption {
    selected: boolean;
}