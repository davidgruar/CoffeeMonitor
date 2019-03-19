using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMonitor.Web.Controllers
{
    using CoffeeMonitor.Data;
    using CoffeeMonitor.Data.Repositories;
    using CoffeeMonitor.Model.ViewModels;
    using CoffeeMonitor.Model;
    using CoffeeMonitor.Model.Extensions;
    using CoffeeMonitor.Model.Messaging;

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
            var dateToRequest = date ?? DateTime.Today;
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
            var currentBatch = this.coffeeRepository.GetAllForDate(DateTime.Today).LastOrDefault();
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
