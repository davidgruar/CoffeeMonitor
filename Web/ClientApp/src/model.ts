import { Moment } from "moment";

export interface CoffeeBatch {
    brewStarted: Moment;
    brewedBy: string;
    initialCups: number;
    percentDecaff: number;
    pourings?: Pouring[];
    currentCups?: number;
}

export interface Pouring {
    when: Moment;
    who: string;
    cups: number;
}
