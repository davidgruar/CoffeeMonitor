namespace CoffeeMonitor.Data.CosmosDb
{
    public class CosmosDbSettings
    {
        public string EndpointUrl { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }

        public bool ConnectViaTcp { get; set; }
    }
}
