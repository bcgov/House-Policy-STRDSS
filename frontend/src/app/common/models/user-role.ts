export interface UserRole {
    userRoleCd: string;
    userRoleNm: string;
    userPrivilegeCds: Array<UserPermission>;
    isReferenced: boolean;
}

export interface UserPermission {
    userPrivilegeCd: string;
    userPrivilegeNm: string;
}