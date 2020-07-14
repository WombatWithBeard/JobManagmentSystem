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
        private readonly IPersistStorage _storage;

        public JobController(IScheduler scheduler, IPersistStorage storage)
        {
            _scheduler = scheduler;
            _storage = storage;
        }

        [HttpPost]
        public IActionResult Create([FromBody] JobDto dto)
        {
            var createJob = new CreateJob(_scheduler, _storage);
            var success = createJob.Create(dto);

            return Ok(success);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] string key)
        {
            var deleteJob = new DeleteJob(_scheduler, _storage);
            var success = deleteJob.Delete(key);

            return Ok(success);
        }

        [HttpGet("/{key}")]
        public IActionResult Get([FromBody] string key)
        {
            var getJob = new GetJob(_storage);
            var job = getJob.GetJobById(key);

            return Ok(job);
        }

        [HttpGet]
        public IActionResult GetAll([FromBody] JobDto dto)
        {
            var jobsList = new GetJobsList(_storage);
            var jobs = jobsList.GetJobs();

            return Ok(jobs);
        }
    }
}