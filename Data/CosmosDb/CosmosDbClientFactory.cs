namespace CoffeeMonitor.Data.CosmosDb
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Extensions.Options;

    public class CosmosDbClientFactory
    {
        private readonly CosmosDbSettings cosmosDbSettings;

        public CosmosDbClientFactory(IOptions<CosmosDbSettings> options)
        {
            this.cosmosDbSettings = options.Value;
        }

        public IDocumentClient Create()
        {
            var uri = new Uri(this.cosmosDbSettings.EndpointUrl);
            var policy = new ConnectionPolicy();
            if (this.cosmosDbSettings.ConnectViaTcp)
            {
                policy.ConnectionMode = ConnectionMode.Direct;
                policy.ConnectionProtocol = Protocol.Tcp;
            }

            var client = new DocumentClient(uri, this.cosmosDbSettings.Key, policy);
            this.Initialize(client).Wait();
            return client;
        }

        private async Task Initialize(IDocumentClient client)
        {
            var database = new Database { Id = this.cosmosDbSettings.DatabaseId };
            await client.CreateDatabaseIfNotExistsAsync(database);
            var partitionKey = new PartitionKeyDefinition() { Paths = { "/PartitionKey" } };
            var collection = new DocumentCollection { Id = this.cosmosDbSettings.CollectionId, PartitionKey = partitionKey };
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(database.Id), collection);
        }
    }
}
