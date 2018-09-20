import axios from "axios";
import * as moment from "moment";
import { CoffeeBatch } from "./model";

const baseUrl = "/api/coffee";

export function createBatch(batch: CoffeeBatch) {
    return axios.post(baseUrl, batch);
}

export async function getBatches(): Promise<CoffeeBatch[]> {
    const response = await axios.get(baseUrl);
    const data: CoffeeBatch[] = response.data;
    return data.map(batch => ({
        ...batch,
        brewStarted: moment(batch.brewStarted)
    }));
}
