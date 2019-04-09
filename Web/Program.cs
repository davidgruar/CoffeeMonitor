using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace CoffeeMonitor.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
