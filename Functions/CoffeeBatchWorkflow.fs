namespace Functions

open System
open System.Net.Http
open System.Threading
open FSharp.Control.Tasks.V2
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open NodaTime
open CoffeeMonitor.Model

module CoffeeBatchWorkflow =
    let BrewRateSecondsPerCup = 5.0 // 55

    [<FunctionName("CoffeeBatchWorkflow")>]
    let runOrchestrator
        ([<OrchestrationTrigger>] context: DurableOrchestrationContextBase)
        (log: ILogger) =
        task {
            let batch = context.GetInput<CoffeeBatch>()

            let brewTime = batch.CurrentCups * BrewRateSecondsPerCup |> Duration.FromSeconds
            let currentTime = context.CurrentUtcDateTime |> Instant.FromDateTimeUtc
            let finishedTime = currentTime + brewTime
            do! context.CallActivityAsync("NotifyBrewStarted", (batch, finishedTime))

            use cancelCoffeeReady = new CancellationTokenSource()
            do! context.CreateTimer(finishedTime.ToDateTimeUtc(), cancelCoffeeReady.Token)

            log.LogInformation("{InstanceId}: Coffee ready!", context.InstanceId);
            do! context.CallActivityAsync("NotifyCoffeeReady", batch)
        }

    [<FunctionName("CoffeeBatchWorkflow_Start")>]
    let start
        ([<HttpTrigger("POST", Route = "coffeebatchworkflow/start")>] req: HttpRequestMessage)
        ([<OrchestrationClient>] client: DurableOrchestrationClient)
        (log: ILogger) =
        task {
            let! batch = req.Content.ReadAsAsync<CoffeeBatch>()
            let! instanceId = client.StartNewAsync ("CoffeeBatchWorkflow", batch)

            log.LogInformation ("Started orchestration with {InstanceId}", instanceId)

            return client.CreateCheckStatusResponse (req, instanceId)
        }

    [<FunctionName("NotifyBrewStarted")>]
    let notifyBrewStarted
        ([<ActivityTrigger>] context: DurableActivityContext)
        (log: ILogger) =
        task {
            let (batch, finishedTime) = context.GetInput<(CoffeeBatch * Instant)>()
            log.LogInformation("{InstanceId}: Coffee will be ready at {FinishedTime}", context.InstanceId, finishedTime);
        }

    [<FunctionName("NotifyCoffeeReady")>]
    let notifyCoffeeReady
        ([<ActivityTrigger>] context: DurableActivityContext)
        (log: ILogger) =
        task {
            let batch = context.GetInput<CoffeeBatch>()
            log.LogInformation("{InstanceId}: Coffee ready!", context.InstanceId);
        }