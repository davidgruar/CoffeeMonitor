using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMonitor.Web.Controllers
{
    using CoffeeMonitor.Data.Repositories;
    using CoffeeMonitor.Model.ViewModels;
    using CoffeeMonitor.Model;
    using CoffeeMonitor.Model.Messaging;

    using NodaTime;

    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : Controller
    {
        private readonly ICoffeeRepository coffeeRepository;

        private readonly ICoffeeBatchWorkflowClient workflowClient;

        public CoffeeController(ICoffeeRepository coffeeRepository, ICoffeeBatchWorkflowClient workflowClient)
        {
            this.coffeeRepository = coffeeRepository;
            this.workflowClient = workflowClient;
        }

        [HttpGet]
        public List<CoffeeBatch> Get(DateTime? date = null)
        {
            var dateToRequest = LocalDate.FromDateTime(date ?? DateTime.UtcNow);
            return this.coffeeRepository.GetAllForDate(dateToRequest);
        }

        [HttpPost]
        public async Task Post(CoffeeBatchCreateModel model)
        {
            var batch = new CoffeeBatch(model.BrewStarted, model.InitialCups)
            {
                BrewedBy = model.BrewedBy,
                PercentDecaff = model.PercentDecaff
            };

            if (this.workflowClient.IsConfigured)
            {
                batch.WorkflowData = await this.workflowClient.Start(batch);
            }

            await this.coffeeRepository.CreateItem(batch);
        }

        [HttpPost("pourings")]
        public async Task<IActionResult> Pour(Pouring pouring)
        {
            var date = LocalDate.FromDateTime(DateTime.UtcNow);
            var currentBatch = this.coffeeRepository.GetAllForDate(date).LastOrDefault();
            if (currentBatch == null)
            {
                return this.Conflict("There is no current batch to pour from.");
            }

            currentBatch.Pourings.Add(pouring);
            await this.coffeeRepository.UpdateItem(currentBatch);
            return this.Ok();
        }
    }
}
