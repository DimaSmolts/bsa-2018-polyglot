
export class AdvancedQuery {
    tags: number[] = [];
    status: translationType = translationType.All;
    searchQuery: string = "";
}

export enum translationType {
    All = 0,
    Untranslated = 1,
    InProgress = 2,
    Done = 3
}
