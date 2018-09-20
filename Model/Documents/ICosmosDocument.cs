namespace CoffeeMonitor.Model.Documents
{
    using Newtonsoft.Json;

    public interface ICosmosDocument
    {
        [JsonProperty("id")]
        string Id { get; }

        string PartitionKey { get; }

        string TypeName { get; }
    }
}
