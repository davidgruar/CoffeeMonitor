using NodaTime;
using System;

namespace CoffeeMonitor.Model
{
    public class Pouring
    {
        public Instant When { get; set; }

        public string Who { get; set; }

        public double Cups { get; set; }
    }
}
