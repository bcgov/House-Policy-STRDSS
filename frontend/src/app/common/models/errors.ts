export interface ErrorBackEnd {
    type: string;
    title: string;
    status: number,
    detail: string;
    instance: string;
    traceId: string;
    errors: ErrorsByEntity;
    statusText?: string;
    message?: string;
}

export interface ErrorsByEntity {
    [key: string]: Array<string>;
}