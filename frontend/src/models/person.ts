export interface Person {
    id: number;
    name: string;
    city: string;
    region: string;
    date: Date;
}

export interface State {
    persons: Person[];
}