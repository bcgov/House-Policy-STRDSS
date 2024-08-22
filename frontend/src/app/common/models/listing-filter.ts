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
        isBusinessLicenceRequired: '' | 'Yes' | 'No';
    };
    community: number;
}