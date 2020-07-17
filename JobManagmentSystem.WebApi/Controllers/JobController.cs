﻿using System.Threading.Tasks;
using JobManagmentSystem.Application;
using Microsoft.AspNetCore.Mvc;

namespace JobManagmentSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _service;

        public JobController(IJobService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JobDto dto)
        {
            var createJob = await _service.ScheduleJobAsync(dto);

            if (!createJob.success)
            {
                //TODO: what about this?
            }

            return Ok(createJob.ToString());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string key)
        {
            var deleteJob = await _service.UncheduleJobAsync(key);

            return Ok(deleteJob.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> ReSchedule([FromBody] JobDto dto)
        {
            var scheduleJob = await _service.RescheduleJobAsync(dto);

            if (!scheduleJob.success)
            {
                //TODO: what about this?
            }

            return Ok(scheduleJob.ToString());
        }

        [HttpGet("/{key}")]
        public async Task<IActionResult> Get([FromBody] string key)
        {
            var getJob = await _service.GetScheduledJobByIdAsync(key);
            if (!getJob.success) return NotFound(getJob.message);

            return Ok(getJob.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _service.GetAllSchedulerJobsAsync();
            if (!jobs.success) return NotFound(jobs.message);

            return Ok(jobs.ToString());
        }
    }
}