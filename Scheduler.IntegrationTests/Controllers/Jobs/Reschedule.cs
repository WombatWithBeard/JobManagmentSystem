﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.WebApi;
using Scheduler.IntegrationTests.Common;
using Xunit;

namespace Scheduler.IntegrationTests.Controllers.Jobs
{
    public class Reschedule : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private TestJobMaker _jobMaker;

        public Reschedule(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _jobMaker = new TestJobMaker();
        }

        [Fact]
        public async Task Schedule_ReturnsSuccessStatusCode()
        {
            var jobUnit = _jobMaker.CreateJobDto(Guid.NewGuid().ToString());
            var content = Utilities.GetRequestContent(jobUnit);

            await _client.PostAsync("api/Job/Schedule", content);

            var response = await _client.PostAsync("api/Job/Reschedule", content);

            response.EnsureSuccessStatusCode();

            var vm = await Utilities.GetResponseContent<Result>(response);

            Assert.True(vm.Success);
        }

        [Fact]
        public async Task Schedule_ReturnsBadRequestsStatusCode()
        {
            var jobUnit = new object();
            var content = Utilities.GetRequestContent(jobUnit);

            var response = await _client.PostAsync("api/Job/Reschedule", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}