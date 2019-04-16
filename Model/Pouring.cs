using NodaTime;
using System;

namespace CoffeeMonitor.Model
{
    public class Pouring
    {
        public Instant When { get; set; } = Instant.FromDateTimeUtc(DateTime.UtcNow);

        public string Who { get; set; }

        public double Cups { get; set; }
    }
}
