export interface HistoryRow {
    id: number;
    entity: string;
    entityId: number;
    field: string;
    oldValue: string;
    newValue: string;
    operation: string;
    updatedTime: string;
}