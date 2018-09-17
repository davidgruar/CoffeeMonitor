namespace CoffeeMonitor.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using CoffeeMonitor.Data.CosmosDb;
    using CoffeeMonitor.Model;
    using CoffeeMonitor.Model.Extensions;
    using Microsoft.Azure.Documents;
    using Microsoft.Extensions.Options;

    public interface ICoffeeRepository : ICosmosRepository<CoffeeBatch>
    {
        List<CoffeeBatch> GetAllForDate(DateTime date);
    }

    public class CoffeeRepository : CosmosRepository<CoffeeBatch>, ICoffeeRepository
    {
        public CoffeeRepository(IDocumentClient client, IOptions<CosmosDbSettings> options) : base(client, options)
        {            
        }

        public List<CoffeeBatch> GetAllForDate(DateTime date) => this.GetAll(date.ToPartitionKey());
    }
}
