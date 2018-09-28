namespace CoffeeMonitor.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CoffeeMonitor.Model.Documents;

    using Newtonsoft.Json;

    public class CoffeeBatch : CosmosDocument
    {
        [JsonConstructor]
        public CoffeeBatch(string id, string partitionKey) : base(id, partitionKey)
        {
        }

        public CoffeeBatch(DateTime brewStarted, int initialCups)
            : base(brewStarted)
        {
            this.BrewStarted = brewStarted;
            this.InitialCups = initialCups;
        }

        public DateTime BrewStarted { get; set; }

        public string BrewedBy { get; set; }

        public int InitialCups { get; set; }

        public int PercentDecaff { get; set; }

        public ICollection<Pouring> Pourings { get; } = new List<Pouring>();

        public double CurrentCups => this.GetCurrentCups();

        private double GetCurrentCups()
        {
            var poured = this.Pourings.Sum(p => p.Cups);
            return this.InitialCups - poured;
        }
    }
}
