namespace CoffeeMonitor.Model
{
    using System;
    using CoffeeMonitor.Model.Documents;

    public class CoffeeBatch : CosmosDocument
    {
        public CoffeeBatch(DateTime brewStarted, int initialVolumeMl)
            : base(brewStarted)
        {
            this.BrewStarted = brewStarted;
            this.InitialVolumeMl = initialVolumeMl;
        }

        public DateTime BrewStarted { get; }

        public string BrewedBy { get; set; }

        public int InitialVolumeMl { get; }

        public int PercentDefaff { get; set; }
    }
}
