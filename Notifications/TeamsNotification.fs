module CoffeeMonitor.Notifications.TeamsNotification

open System.Text
open Newtonsoft.Json
open System.Net.Http

type LinkTarget = { Os: string; Uri: string; }

type NotificationAction = { [<JsonProperty("@type")>] Type: string; Name: string; Targets: LinkTarget list }

type Notification = { Title: string; Text: string; PotentialAction: NotificationAction list }

let createLink uri text =
    { Type = "OpenUri"; Name = text; Targets = [{ Os = "default"; Uri = uri }] }

let create link title text =
    { Title = title; Text = text; PotentialAction = [link]}

let httpClient = new HttpClient()

let send (webhook: string) (notification: Notification) =
    let json = JsonConvert.SerializeObject notification
    async {
        let content = new StringContent(json, Encoding.UTF8, "application/json")
        return! httpClient.PostAsync(webhook, content) |> Async.AwaitTask
    }
