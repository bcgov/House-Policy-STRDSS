export interface LocalGovernment {
    businessLicenceFormatTxt: string;
    localGovernmentType: string;
    localGovernmentTypeNm: string;
    organizationCd: string;
    organizationId: number;
    organizationNm: string;
    organizationType: string;
    updDtm: string;
    jurisdictions: Array<Jurisdiction>;
}

export interface LocalGovernmentUpdate {
    organizationId: number;
    organizationNm: string;
    localGovernmentType: string;
    businessLicenceFormatTxt: string;
    updDtm: string;
}

export interface Jurisdiction {
    organizationNm: string;
    organizationId: number;
    shapeFileId: string;
    isPrincipalResidenceRequired: boolean;
    isStrProhibited: boolean;
    isStraaExempt: boolean;
    isBusinessLicenceRequired: boolean;
    economicRegionDsc: string;
    managingOrganizationId: number;
    updDtm: string;
    isActive: boolean;
}

export interface JurisdictionUpdate {
    organizationId: number;
    managingOrganizationId: number;
    isPrincipalResidenceRequired: boolean;
    isStrProhibited: boolean;
    isStraaExempt: boolean;
    isBusinessLicenceRequired: boolean;
    economicRegionDsc: string;
    updDtm: string;
    isActive: boolean;
}