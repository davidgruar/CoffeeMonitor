using System;

namespace CoffeeMonitor.Model.ViewModels
{
    public class CoffeeBatchCreateModel
    {
        public DateTime BrewStarted { get; set; }

        public string BrewedBy { get; set; }

        public int InitialCups { get; set; }

        public int PercentDecaff { get; set; }
    }
}
