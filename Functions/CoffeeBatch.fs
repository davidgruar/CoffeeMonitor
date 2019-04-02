module CoffeeMonitor.Functions.CoffeeBatch

open NodaTime

let BrewRateSecondsPerCup = 55.0

let brewTime cups = cups * BrewRateSecondsPerCup |> Duration.FromSeconds

