module CoffeeMonitor.Functions.Tests.CoffeeBatchWorkflowTests

open Xunit
open FsUnit
open CoffeeMonitor.Functions.CoffeeBatchWorkflow
open CoffeeMonitor.Model
open Microsoft.Azure.WebJobs
open FSharp.Control.Tasks.V2
open Microsoft.Extensions.Logging
open System
open NSubstitute
open System.Threading
open System.Threading.Tasks
open NodaTime

type TaskCompletionSource() =
    inherit TaskCompletionSource<int>()
    member this.SetCompleted() = this.SetResult 0

let logger = Substitute.For<ILogger>()

let returns<'T> (v: 'T) x = x.Returns(v) |> ignore

let createContext<'T> input =
    let context = Substitute.For<DurableOrchestrationContextBase>()
    context.GetInput<'T>() |> returns input
    context

[<Fact>]
let ``Should notify that the brew has started with correct finish time`` () =
    task {
        let brewTime = DateTime(2019, 3, 10, 13, 0, 0, DateTimeKind.Utc)
        let batch = CoffeeBatch(brewTime |> Instant.FromDateTimeUtc, 12)
        let context = createContext batch

        do! runOrchestrator context logger

        do! context.Received().CallActivityAsync("NotifyBrewStarted", { Batch = batch; FinishedTime = "13:11" })
    }

[<Fact>]
let ``Should notify that coffee is ready when the timer expires`` () =
    task {
        let brewTime = DateTime(2019, 3, 10, 13, 0, 0, DateTimeKind.Utc)
        let batch = CoffeeBatch(brewTime |> Instant.FromDateTimeUtc, 12)
        let context = createContext batch
        let tcs = TaskCompletionSource()
        context.CreateTimer(brewTime.AddMinutes 11.0, Arg.Any<CancellationToken>()) |> returns (tcs.Task :> Task)

        let task = runOrchestrator context logger
        do! context.DidNotReceive().CallActivityAsync("NotifyCoffeeReady", batch)

        tcs.SetCompleted()
        do! context.Received().CallActivityAsync("NotifyCoffeeReady", batch)
    }