namespace CoffeeMonitor.Data.CosmosDb
{
    using Microsoft.Azure.Documents;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddCosmosDb(this IServiceCollection services)
        {
            //services.AddOptions<CosmosDbSettings>("CosmosDb");
            services.AddTransient<CosmosDbClientFactory>();
            services.AddSingleton<IDocumentClient>(s => s.GetService<CosmosDbClientFactory>().Create());
        }
    }
}