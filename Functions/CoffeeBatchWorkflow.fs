module CoffeeMonitor.Functions.CoffeeBatchWorkflow

open System
open System.Globalization
open System.Net.Http
open System.Threading
open FSharp.Control.Tasks.V2
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open NodaTime
open CoffeeMonitor.Model
open CoffeeMonitor.Notifications

let ukTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London")
let gbCulture = CultureInfo("en-GB")

type BatchStartedData = { Batch: CoffeeBatch; FinishedTime: string }

[<FunctionName("CoffeeBatchWorkflow")>]
let runOrchestrator
    ([<OrchestrationTrigger>] context: DurableOrchestrationContextBase)
    (log: ILogger) =
    task {
        let batch = context.GetInput<CoffeeBatch>()

        let brewTime = CoffeeBatch.brewTime batch.CurrentCups
        let finishedTime = batch.BrewStarted + brewTime
        let localFinishedTime = finishedTime.InZone(ukTimeZone)

        do! context.CallActivityAsync("NotifyBrewStarted", { Batch = batch; FinishedTime = localFinishedTime.ToString("HH:mm", gbCulture) })

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


let sendToTeams (log: ILogger) instanceId (notification: TeamsNotification.Notification) =
    async {
        let enabled = Environment.GetEnvironmentVariable("Notifications:Teams:Enabled") |> Convert.ToBoolean
        if not enabled then
            log.LogInformation("Teams notifications disabled. Would send {@Notification}", notification)
        else
            let webhook = Environment.GetEnvironmentVariable("Notifications:Teams:Webhook");
            log.LogInformation("Sending Teams notification {@Notification} to webhook {Webhook}", notification, webhook)
            let! response = TeamsNotification.send webhook notification

            if response.IsSuccessStatusCode
            then log.LogInformation("{InstanceId}: Successfully delivered notification '{NotificationTitle}'", instanceId, notification.Title)
            else log.LogError("{InstanceId}: Notification delivery failed with code {StatusCode}, reason {Reason}", instanceId, response.StatusCode, response.ReasonPhrase)
    }

[<FunctionName("NotifyBrewStarted")>]
let notifyBrewStarted
    ([<ActivityTrigger>] context: DurableActivityContext)
    (log: ILogger) =
    task {
        let data = context.GetInput<BatchStartedData>()
        log.LogInformation("{InstanceId}: Notifying brew started: {@Data}", context.InstanceId, data);
        let batch = data.Batch
        let link = TeamsNotification.createLink "https://coffeelovers.azurewebsites.net" "Open app"
        let message = sprintf "%s brewed %i cups of %s. Ready at %s." batch.BrewedBy batch.InitialCups (batch.GetCaffDescription()) data.FinishedTime
        let notification = TeamsNotification.create link "Coffee brewing" message

        do! sendToTeams log context.InstanceId notification
    }

[<FunctionName("NotifyCoffeeReady")>]
let notifyCoffeeReady
    ([<ActivityTrigger>] context: DurableActivityContext)
    (log: ILogger) =
    task {
        log.LogInformation("{InstanceId}: Coffee ready", context.InstanceId);
        let link = TeamsNotification.createLink "https://coffeelovers.azurewebsites.net" "Open app"
        let notification = TeamsNotification.create link "Coffee ready" "Drink it while it's hot!"

        do! sendToTeams log context.InstanceId notification
    }
