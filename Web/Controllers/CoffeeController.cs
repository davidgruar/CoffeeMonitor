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

    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : Controller
    {
        private readonly ICoffeeRepository coffeeRepository;

        public CoffeeController(ICoffeeRepository coffeeRepository)
        {
            this.coffeeRepository = coffeeRepository;
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
            var batch = new CoffeeBatch(model.BrewStarted, model.InitialVolumeMl)
            {
                BrewedBy = model.BrewedBy,
                PercentDefaff = model.PercentDefaff
            };
            await this.coffeeRepository.CreateItem(batch);
        }
    }
}
