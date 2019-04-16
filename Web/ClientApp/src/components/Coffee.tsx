import * as React from 'react';
import { Button, Modal, Segment } from "semantic-ui-react";
import * as coffeeApi from "../coffeeApi";
import { CoffeeBatch, PouringCreateModel } from '../model';
import { CoffeeBatchForm } from './CoffeeBatchForm';
import { CoffeeStatus } from './CoffeeStatus';
import { PourCoffeeForm } from "./PourCoffeeForm";

interface CoffeeState {
    currentBatch: CoffeeBatch | undefined;
    modalOpen: boolean;
}

export class Coffee extends React.Component<{}, CoffeeState> {
    private refreshInterval: any;

    constructor(props: any) {
        super(props);
        this.state = {
            currentBatch: undefined,
            modalOpen: false
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
                    <div>
                        <Modal open={this.state.modalOpen} trigger={<Button>Brew some coffee</Button>} onOpen={this.openModal} onClose={this.closeModal}>
                            <Modal.Header>Brew some coffee</Modal.Header>
                            <Modal.Content>
                                <CoffeeBatchForm onSubmit={this.submitBatch} />
                            </Modal.Content>
                        </Modal>
                    </div>
                </Segment>
                {this.state.currentBatch && this.state.currentBatch.currentCups! > 0 &&
                    <Segment>
                        <h2>Pour a cup</h2>
                        <PourCoffeeForm
                            currentCups={this.state.currentBatch.currentCups || 0}
                            onSubmit={this.pour} />
                    </Segment>}
            </div>
        );
    }

    private async loadCurrentBatch() {
        const batches = await coffeeApi.getBatches();
        const currentBatch = batches[batches.length - 1];
        this.setState({ currentBatch });
    }

    private openModal = () => this.setState({ modalOpen: true });
    
    private closeModal = () => this.setState({ modalOpen: false });

    private submitBatch = async (batch: CoffeeBatch) => {
        this.closeModal();
        await coffeeApi.createBatch(batch);
        await this.loadCurrentBatch();
    }

    private pour = async (pouring: PouringCreateModel) => {
        await coffeeApi.pour(pouring);
        await this.loadCurrentBatch();
    }
}
