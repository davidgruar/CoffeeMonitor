import * as React from "react";
import { CoffeeBatch } from "../model";

interface CoffeeStatusProps {
    batch: CoffeeBatch | undefined;
}

const CoffeeStatus: React.SFC<CoffeeStatusProps> = ({ batch }) => {
    if (!batch) {
        return <div>Looks like no one has made any coffee today.</div>
    }

    return (
        <div>
            <ul>
                <li>Brew started: <span title={batch.brewStarted.format("HH:mm")}>{batch.brewStarted.fromNow()}</span></li>
                <li>Brewed by: {batch.brewedBy}</li>
                <li>Initial cups: {batch.initialCups}</li>
                <li>Decaff: {batch.percentDecaff}%</li>
            </ul>
        </div>
    )
}

export { CoffeeStatus };
