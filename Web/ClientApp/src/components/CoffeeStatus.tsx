import * as pluralize from "pluralize";
import * as React from "react";
import { Progress } from "semantic-ui-react";
import { CoffeeBatch } from "../model";
import { LogEntry } from "./LogEntry";

interface CoffeeStatusProps {
    batch: CoffeeBatch | undefined;
}

const CoffeeStatus: React.SFC<CoffeeStatusProps> = ({ batch }) => {
    if (!batch) {
        return <div>Looks like no one has made any coffee today.</div>
    }

    return (
        <div>
            <h3>{formatDecaff(batch.percentDecaff)} - brewed {batch.brewStarted.fromNow()} - {cupCount(batch.currentCups!)} left</h3>
            <Progress
                value={batch.currentCups}
                total={12}
                color="brown"/>
            <ul>
                <LogEntry timestamp={batch.brewStarted}>{batch.brewedBy} started the brew</LogEntry>
                {batch.pourings && batch.pourings.map(pouring => 
                    <LogEntry timestamp={pouring.when} key={pouring.when.toString()}>{pouring.who} poured {cupCount(pouring.cups)}</LogEntry>
                )}
            </ul>
            
        </div>
    )
}

function formatDecaff(percent: number) {
    switch (percent) {
        case 100: return "Decaff";
        case 0: return "Caff";
        default: return `${percent}% decaff`
    }
}

function cupCount(cups: number) {
    return `${cups} ${pluralize("cup", cups)}`;
}

export { CoffeeStatus };
