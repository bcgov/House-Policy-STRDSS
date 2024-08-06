export interface ListingUploadHistoryRecord {
    uploadedBy?: string;

    status?: 'Processed' | 'Pending';
    errors: number;
    success: number;
    processed: number;
    familyNm: string;
    givenNm: string;
    organizationNm: string;
    providingOrganizationId: number;
    reportPeriodYM: string;
    total: number;
    updDtm: string;
    uploadDeliveryId: number;
    uploadDeliveryType?: string;
}