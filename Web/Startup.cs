using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeMonitor.Web
{
    using CoffeeMonitor.Data.CosmosDb;
    using CoffeeMonitor.Data.Repositories;
    using CoffeeMonitor.Model.Messaging;

    public class Startup
    {
        private const string ClientAppRoot = "ClientApp";

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = $"{ClientAppRoot}/build";
            });

            services.Configure<CosmosDbSettings>(this.Configuration.GetSection("CosmosDb"));
            services.Configure<MessagingSettings>(this.Configuration.GetSection("Messaging"));

            services.AddCosmosDb();
            services.AddHttpClient();


            services.AddScoped(typeof(ICosmosRepository<>), typeof(CosmosRepository<>));
            services.AddScoped<ICoffeeRepository, CoffeeRepository>();
            services.AddScoped<ICoffeeBatchWorkflowClient, CoffeeBatchWorkflowClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = ClientAppRoot;

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
