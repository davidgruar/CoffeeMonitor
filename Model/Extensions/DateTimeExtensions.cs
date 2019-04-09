using NodaTime;
using System;
using System.Globalization;

namespace CoffeeMonitor.Model.Extensions
{
    public static class NodaTimeExtensions
    {
        public static string ToPartitionKey(this Instant instant) => instant.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public static string ToPartitionKey(this LocalDate instant) => instant.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
