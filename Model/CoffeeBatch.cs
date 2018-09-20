namespace CoffeeMonitor.Model
{
    using System;
    using CoffeeMonitor.Model.Documents;

    public class CoffeeBatch : CosmosDocument
    {
        public CoffeeBatch(DateTime brewStarted, int initialCups)
            : base(brewStarted)
        {
            this.BrewStarted = brewStarted;
            this.InitialCups = initialCups;
        }

        public DateTime BrewStarted { get; }

        public string BrewedBy { get; set; }

        public int InitialCups { get; }

        public int PercentDecaff { get; set; }
    }
}
