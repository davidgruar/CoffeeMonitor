namespace CoffeeMonitor.Model.Messaging
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

    public interface ICoffeeBatchWorkflowClient
    {
        bool IsConfigured { get; }

        Task<DurableFunctionInstanceData> Start(CoffeeBatch batch);
    }

    public class CoffeeBatchWorkflowClient : ICoffeeBatchWorkflowClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly MessagingSettings messagingSettings;

        public CoffeeBatchWorkflowClient(IOptions<MessagingSettings> messagingOptions, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.messagingSettings = messagingOptions.Value;
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(this.messagingSettings.FunctionsEndpoint);

        public async Task<DurableFunctionInstanceData> Start(CoffeeBatch batch)
        {
            if (!this.IsConfigured)
            {
                throw new InvalidOperationException("Workflow endpoint is not configured");
            }

            var uri = this.GetEndpoint("start");
            var content = new StringContent(JsonConvert.SerializeObject(batch), Encoding.UTF8, "application/json");
            var client = this.httpClientFactory.CreateClient();
            var response = await client.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DurableFunctionInstanceData>(json);
            return result;
        }

        private Uri GetEndpoint(string action)
        {
            var builder = new UriBuilder(this.messagingSettings.FunctionsEndpoint);
            builder.Path += $"coffeebatchworkflow/{action}";
            return builder.Uri;
        }
    }
}