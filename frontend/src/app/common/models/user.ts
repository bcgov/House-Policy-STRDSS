export interface User {
    id?: number;
    username: string;
    password?: string;
    lastName: string;
    streetAddress: string;
    city: string;
    province: string;
    postalCode: string;
    phoneNumber: string;
    created?: string;
}

export interface AuthenticatedUser {
    username: string;
    lastName: string;
    role: string;
    token: string;
}