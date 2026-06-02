export interface TaskDto{
    id: string;
    title: string;
    description: string;
    isCompleted: boolean;
    createdAt: Date;
}

export interface CreateTaskRequest{
    title: string;
    description: string;
}