import * as React from 'react';
import { Segment } from "semantic-ui-react";
import * as coffeeApi from "../coffeeApi";
import { CoffeeBatch } from '../model';
import { CoffeeBatchForm } from './CoffeeBatchForm';
import { CoffeeStatus } from './CoffeeStatus';

interface CoffeeState {
    currentBatch: CoffeeBatch | undefined;
}

export class Coffee extends React.Component<{}, CoffeeState> {
    private refreshInterval: any;

    constructor(props: any) {
        super(props);
        this.state = {
            currentBatch: undefined
        };
    }

    public componentDidMount() {
        this.loadCurrentBatch();
        this.refreshInterval = setInterval(() => this.loadCurrentBatch(), 60000);
    }

    public componentWillUnmount() {
        clearInterval(this.refreshInterval);
    }

    public render() {
        return (
            <div>
                <Segment>
                    <h2>Coffee pot status</h2>
                    <CoffeeStatus batch={this.state.currentBatch} />
                </Segment>
                <Segment>
                    <h2>Brew some coffee</h2>
                    <CoffeeBatchForm onSubmit={this.submitBatch} />
                </Segment>
            </div>
        );
    }

    private async loadCurrentBatch() {
        const batches = await coffeeApi.getBatches();
        const currentBatch = batches[batches.length - 1];
        this.setState({ currentBatch });
    }

    private submitBatch = async (batch: CoffeeBatch) => {
        await coffeeApi.createBatch(batch);
        await this.loadCurrentBatch();
    }
}
