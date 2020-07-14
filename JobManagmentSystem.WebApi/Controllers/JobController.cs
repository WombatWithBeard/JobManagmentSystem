using JobManagmentSystem.Application;
using JobManagmentSystem.Application.CRUD;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagmentSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JobController : ControllerBase
    {
        private readonly IScheduler _scheduler;

        public JobController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        [HttpPost]
        public IActionResult Create([FromBody] JobDto dto)
        {
            var createJob = new CreateJob(_scheduler);
            var success = createJob.Create(dto);

            return Ok(success);
        }
    }
}