import axios from "axios";
import moment from "moment";
import { CoffeeBatch, PouringCreateModel } from "./model";

const baseUrl = "/api/coffee";

export function createBatch(batch: CoffeeBatch) {
    return axios.post(baseUrl, batch);
}

export async function getBatches(): Promise<CoffeeBatch[]> {
    const response = await axios.get(baseUrl);
    const data: CoffeeBatch[] = response.data;
    return data.map(batch => ({
        ...batch,
        brewStarted: moment(batch.brewStarted),
        pourings: batch.pourings && batch.pourings.map(pouring => ({
            ...pouring,
            when: moment(pouring.when)
        }))
    }));
}

export function pour(pouring: PouringCreateModel) {
    return axios.post(`${baseUrl}/pourings`, pouring);
}
