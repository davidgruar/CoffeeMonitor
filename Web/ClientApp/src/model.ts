import { Moment } from "moment";

export interface CoffeeBatch {
    brewStarted: Moment;
    brewedBy: string;
    initialCups: number;
    percentDecaff: number;
}
