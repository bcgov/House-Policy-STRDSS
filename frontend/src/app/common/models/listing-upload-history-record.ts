export interface ListingUploadHistoryRecord {
    id: number;
    platform: string;
    reportedDate: string;
    status: 'Processed' | 'Pending';
    totalRecords: number;
    totalSuccess: number;
    totalErrors: number;
    uploadedDate: string;
    uploadedBy: string;
}