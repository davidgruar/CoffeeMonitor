namespace CoffeeMonitor.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CoffeeMonitor.Data.CosmosDb;
    using CoffeeMonitor.Model.Documents;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Extensions.Options;

    public interface ICosmosRepository<T>
        where T : ICosmosDocument
    {
        T GetItem(string id, string partitionKey);

        Task CreateItem(T item);

        Task UpdateItem(T item);

        List<T> GetAll(string partitionKey);
    }

    public class CosmosRepository<T> : ICosmosRepository<T>
        where T : ICosmosDocument
    {
        private readonly IDocumentClient client;

        private readonly CosmosDbSettings settings;

        private readonly Uri collectionUri;

        private static readonly string TypeName = typeof(T).Name;

        public CosmosRepository(IDocumentClient client, IOptions<CosmosDbSettings> options)
        {
            this.client = client;
            this.settings = options.Value;
            this.collectionUri = UriFactory.CreateDocumentCollectionUri(this.settings.DatabaseId, this.settings.CollectionId);
        }

        public List<T> GetAll(string partitionKey)
        {
            var query = this.CreateQuery().Where(d => d.PartitionKey == partitionKey);
            return query.ToList();
        }

        public T GetItem(string id, string partitionKey)
        {
            var query = this.CreateQuery().Where(d => d.Id == id && d.PartitionKey == partitionKey);
            return query.ToList().SingleOrDefault();
        }

        public Task CreateItem(T item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                throw new Exception("Item must have an ID");
            }
            if (string.IsNullOrEmpty(item.PartitionKey))
            {
                throw new Exception("Item must have a partition key");
            }
            return this.client.CreateDocumentAsync(this.collectionUri, item);
        }

        public async Task UpdateItem(T item)
        {
            var uri = UriFactory.CreateDocumentUri(this.settings.DatabaseId, this.settings.CollectionId, item.Id);
            var response = await this.client.ReplaceDocumentAsync(uri, item);
        }

        private IQueryable<T> CreateQuery()
        {
            return this.client.CreateDocumentQuery<T>(this.collectionUri).Where(d => d.TypeName == TypeName);
        }
    }
}
