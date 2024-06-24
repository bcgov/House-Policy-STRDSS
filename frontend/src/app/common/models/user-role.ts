export interface UserRole {
    userRoleCd: string;
    userRoleDsc: string;
    userRoleNm: string;
    permissions: Array<UserPermission>;
    isReferenced?: boolean;
    updDtm?: string;
}

export interface UserPermission {
    userPrivilegeCd: string;
    userPrivilegeNm: string;
}

export interface UserPermissionSelectable extends UserPermission {
    selected: boolean;
}