export interface ListingFilter {
    byLocation: {
        isPrincipalResidenceRequired: '' | 'Yes' | 'No';
        isBusinessLicenceRequired: '' | 'Yes' | 'No';
    };
    community: number;
}
