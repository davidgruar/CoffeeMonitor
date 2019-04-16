import * as React from "react";
import { Form, InputOnChangeData } from "semantic-ui-react";
import { PouringCreateModel } from "../model";

const userNameKey = "coffeeMonitor.userName";

interface PourCoffeeFormProps {
    currentCups: number;
    onSubmit: (pouring: PouringCreateModel) => Promise<void>;
}

interface PourCoffeeFormState {
    who: string;
    cups: number;
}

class PourCoffeeForm extends React.Component<PourCoffeeFormProps, PourCoffeeFormState> {
    constructor(props: any) {
        super(props);
        this.state = {
            cups: 2,
            who: localStorage.getItem(userNameKey) || ""
        };
    }
    public render() {
        return (
            <Form onSubmit={this.onSubmit}>
                <Form.Input
                    required
                    label="Your Name"
                    value={this.state.who}
                    onChange={this.setValue("who")} />
                <Form.Input
                    required
                    label="Your mug size (cups)"
                    value={this.state.cups}
                    type="number"
                    min="0"
                    max={this.props.currentCups}
                    step="any"
                    onChange={this.setValue("cups")} />
                <Form.Button type="submit">I poured some</Form.Button>
            </Form>
        )
    }

    private onSubmit = () => {
        const pouring: PouringCreateModel = {
            cups: this.state.cups,
            who: this.state.who
        }
        this.props.onSubmit(pouring);
        localStorage.setItem(userNameKey, this.state.who);
    }

    private setValue<P extends keyof PourCoffeeFormState>(key: P) {
        return (event: any, data: InputOnChangeData) => {
            const { value } = data;
            this.setState({[key]: value} as any);
        }
    }
}

export { PourCoffeeForm };
