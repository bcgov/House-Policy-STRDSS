export interface ListingUploadHistoryRecord {
    totalSuccess?: number | 'N/A';
    totalRecords?: number | 'N/A';
    totalErrors?: number | 'N/A';
    status?: 'Processed' | 'Pending';
    uploadedBy?: string;

    errors: number;
    processed: number;
    familyNm: string;
    givenNm: string;
    organizationNm: string;
    providingOrganizationId: number;
    reportPeriodYM: string;
    total: number;
    updDtm: string;
    uploadDeliveryId: number;
}