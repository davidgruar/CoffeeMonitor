﻿namespace CoffeeMonitor.Model.Documents
{
    using System;
    using CoffeeMonitor.Model.Extensions;
    using Newtonsoft.Json;
    using NodaTime;

    public abstract class CosmosDocument : ICosmosDocument
    {
        protected CosmosDocument(string partitionKey)
            : this(Guid.NewGuid().ToString(), partitionKey)
        {
        }

        protected CosmosDocument(Instant partitionDate)
            : this(Guid.NewGuid().ToString(), partitionDate)
        {
        }

        protected CosmosDocument(string id, string partitionKey)
        {
            this.Id = id;
            this.PartitionKey = partitionKey;
        }

        protected CosmosDocument(string id, Instant partitionDate)
            : this(id, partitionDate.ToPartitionKey())
        {
        }

        [JsonProperty("id")]
        public string Id { get; }

        public string PartitionKey { get; }

        public string TypeName => this.GetType().Name;
    }
}
