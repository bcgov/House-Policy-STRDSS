export interface ListingFilter {
    byStatus: {
        reassigned?: boolean;
        takedownComplete?: boolean;
        new?: boolean;
        active?: boolean;
        inactive?: boolean;
    };
    byLocation: {
        isPrincipalResidenceRequired: '' | 'Yes' | 'No';
        isBusinessLicenseRequired: '' | 'Yes' | 'No';
    };
    community: number;
}