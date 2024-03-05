export interface NotificationMessage {
    id: number;
    title: string;
    entity: string;
    entityId: number;
    isRead: boolean;
    userId: number;
}