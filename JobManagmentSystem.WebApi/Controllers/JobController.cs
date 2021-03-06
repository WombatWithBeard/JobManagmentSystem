﻿using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Application;
using JobManagmentSystem.Application.Common.Interfaces;
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
        public async Task<IActionResult> Schedule([FromBody] JobDto dto)
        {
            var createJob = await _service.ScheduleJobAsync(dto);

            return Ok(JsonSerializer.Serialize(createJob));
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Unschedule(string key)
        {
            var deleteJob = await _service.UncheduleJobAsync(key);

            return Ok(JsonSerializer.Serialize(deleteJob));
        }

        [HttpPost]
        public async Task<IActionResult> ReSchedule([FromBody] JobDto dto)
        {
            var scheduleJob = await _service.RescheduleJobAsync(dto);

            return Ok(JsonSerializer.Serialize(scheduleJob));
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var getJob = await _service.GetJobByIdAsync(key);

            return Ok(JsonSerializer.Serialize(getJob));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _service.GetAllJobsAsync();

            return Ok(JsonSerializer.Serialize(jobs));
        }
    }
}