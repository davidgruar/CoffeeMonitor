import * as moment from "moment";
import * as React from "react";
import { Form, InputOnChangeData } from "semantic-ui-react";
import { CoffeeBatch } from "../model";

const userNameKey = "coffeeMonitor.userName";

interface CoffeeBatchFormProps {
    onSubmit: (batch: CoffeeBatch) => Promise<void>;
}

interface CoffeeBatchFormState {
    brewStarted: string;
    brewedBy: string;
    cups: number;
    percentDecaff: number;
}

class CoffeeBatchForm extends React.Component<CoffeeBatchFormProps, CoffeeBatchFormState> {
    constructor(props: any) {
        super(props);
        this.state = {
            brewStarted: moment().format("HH:mm"),
            brewedBy: localStorage.getItem(userNameKey) || "",
            cups: 12,
            percentDecaff: 0            
        };
    }
    public render() {
        return (
            <Form onSubmit={this.onSubmit}>
                <Form.Input
                    required
                    label="Your Name"
                    value={this.state.brewedBy}
                    onChange={this.setValue("brewedBy")} />
                <Form.Input
                    required
                    label="Brew started"
                    value={this.state.brewStarted}
                    type="time"
                    min="1"
                    max="12"
                    onChange={this.setValue("brewStarted")} />
                <Form.Input
                    required
                    label="Cups"
                    value={this.state.cups}
                    type="number"
                    min="1"
                    max="12"
                    onChange={this.setValue("cups")} />
                <Form.Input
                    required
                    label="Percent Decaff"
                    value={this.state.percentDecaff}
                    type="number"
                    min="0"
                    max="100"
                    onChange={this.setValue("percentDecaff")} />
                <Form.Button type="submit">It's brewing</Form.Button>
            </Form>
        )
    }

    private onSubmit = () => {
        const time = moment.duration(this.state.brewStarted);
        const batch: CoffeeBatch = {
            brewStarted: moment().startOf("day").add(time),
            brewedBy: this.state.brewedBy,
            initialCups: this.state.cups,
            percentDecaff: this.state.percentDecaff
        };
        this.props.onSubmit(batch);
        localStorage.setItem(userNameKey, this.state.brewedBy);
    }

    private setValue<P extends keyof CoffeeBatchFormState>(key: P) {
        return (event: any, data: InputOnChangeData) => {
            const { value } = data;
            this.setState({[key]: value} as any);
        }
    }
}

export { CoffeeBatchForm };
