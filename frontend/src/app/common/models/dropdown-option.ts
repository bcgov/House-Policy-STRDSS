export interface DropdownOption {
    label: string,
    value: any,
}
export interface DropdownOptionSelectable extends DropdownOption {
    selected: boolean;
}