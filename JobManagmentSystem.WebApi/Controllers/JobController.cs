using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Application;
using Microsoft.AspNetCore.Mvc;

namespace JobManagmentSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JobController : ControllerBase
    {
        private readonly JobManagement _management;

        public JobController(JobManagement management)
        {
            _management = management;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JobDto dto)
        {
            var createJob = await _management.CreateJobAsync(dto);

            if (!createJob.success)
            {
                //TODO: what about this?
            }

            return Ok(JsonSerializer.Serialize(createJob));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string key)
        {
            var deleteJob = await _management.DeleteJobAsync(key);

            return Ok(JsonSerializer.Serialize(deleteJob));
        }

        [HttpPost]
        public async Task<IActionResult> ReSchedule([FromBody] JobDto dto)
        {
            var scheduleJob = await _management.ReScheduleJobAsync(dto);

            if (!scheduleJob.success)
            {
                //TODO: what about this?
            }

            return Ok(JsonSerializer.Serialize(scheduleJob));
        }

        [HttpGet("/{key}")]
        public async Task<IActionResult> Get([FromBody] string key)
        {
            var getJob = await _management.GetJobAsync(key);
            if (!getJob.success) return NotFound(getJob.message);

            return Ok(JsonSerializer.Serialize(getJob.job));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobsList = await _management.GetJobsListAsync();
            if (!jobsList.success) return NotFound(jobsList.message);

            return Ok(JsonSerializer.Serialize(jobsList.jobs));
        }
    }
}