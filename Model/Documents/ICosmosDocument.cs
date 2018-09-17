namespace CoffeeMonitor.Model.Documents
{
    using Newtonsoft.Json;

    public interface ICosmosDocument
    {
        [JsonProperty("id")]
        string Id { get; }

        [JsonProperty("partitionKey")]
        string PartitionKey { get; }

        string TypeName { get; }
    }
}
