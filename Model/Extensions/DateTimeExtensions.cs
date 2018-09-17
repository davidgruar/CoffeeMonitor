using System;

namespace CoffeeMonitor.Model.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPartitionKey(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");
    }
}
