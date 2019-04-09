namespace CoffeeMonitor.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using CoffeeMonitor.Data.CosmosDb;
    using CoffeeMonitor.Model;
    using CoffeeMonitor.Model.Extensions;
    using Microsoft.Azure.Documents;
    using Microsoft.Extensions.Options;

    using NodaTime;

    public interface ICoffeeRepository : ICosmosRepository<CoffeeBatch>
    {
        List<CoffeeBatch> GetAllForDate(LocalDate date);
    }

    public class CoffeeRepository : CosmosRepository<CoffeeBatch>, ICoffeeRepository
    {
        public CoffeeRepository(IDocumentClient client, IOptions<CosmosDbSettings> options) : base(client, options)
        {            
        }

        public List<CoffeeBatch> GetAllForDate(LocalDate date) => this.GetAll(date.ToPartitionKey());
    }
}
