import { CommonCode } from "./common-code.model";

export interface Applicant {
    id: number;
    lastName: string;
    streetAddress: string;
    city: string;
    province: string;
    postalCode: string;
    phoneNumber: string;
    role: CommonCode;
}

export interface StrApplication {
    id: number;
    applicantId: number;
    streetAddress: string;
    city: string;
    province: string;
    postalCode: string;
    zoningTypeId: number;
    squareFootage: number;
    strAffiliateId: number;
    isOwnerPrimaryResidence: boolean;
    complianceStatusId: number;
    applicant: Applicant;
    zoningType: CommonCode;
    strAffiliate: CommonCode;
    complianceStatus: CommonCode;
    longitude: number;
    latitude: number;
    dateCreated: string;
}

export interface PostApplication {
    streetAddress: string;
    city: string;
    province: string;
    postalCode: string;
    zoningTypeId: number;
    squareFootage: number;
    strAffiliateId: number;
    isOwnerPrimaryResidence: boolean;
}

export interface PutApplication extends PostApplication {
    id: number;
    complianceStatusId: number;
    applicantId: number
}