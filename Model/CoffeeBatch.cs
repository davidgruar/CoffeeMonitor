namespace CoffeeMonitor.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using CoffeeMonitor.Model.Documents;
    using CoffeeMonitor.Model.Messaging;

    using Newtonsoft.Json;
    using NodaTime;

    public class CoffeeBatch : CosmosDocument
    {
        [JsonConstructor]
        public CoffeeBatch(string id, string partitionKey) : base(id, partitionKey)
        {
        }

        public CoffeeBatch(Instant brewStarted, int initialCups)
            : base(brewStarted)
        {
            this.BrewStarted = brewStarted;
            this.InitialCups = initialCups;
        }

        public Instant BrewStarted { get; set; }

        public string BrewedBy { get; set; }

        public int InitialCups { get; set; }

        public int PercentDecaff { get; set; }

        public ICollection<Pouring> Pourings { get; } = new List<Pouring>();

        public double CurrentCups => this.GetCurrentCups();

        public DurableFunctionInstanceData WorkflowData { get; set; }

        public string GetCaffDescription()
        {
            switch (this.PercentDecaff)
            {
                case 0:
                    return "caff";
                case 100:
                    return "decaff";
                default:
                    return $"{this.PercentDecaff}% decaff";
            }
        }
        private double GetCurrentCups()
        {
            var poured = this.Pourings.Sum(p => p.Cups);
            return this.InitialCups - poured;
        }
    }
}
