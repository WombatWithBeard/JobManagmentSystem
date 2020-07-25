using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.WebApi;
using Scheduler.IntegrationTests.Common;
using Xunit;

namespace Scheduler.IntegrationTests.Controllers.Jobs
{
    public class Unschedule : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        private TestJobMaker _jobMaker;

        public Unschedule(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jobMaker = new TestJobMaker();
        }

        [Fact]
        public async Task Unschedule_ReturnsSuccessStatusCode()
        {
            var jobUnit = _jobMaker.CreateJobDto(Guid.NewGuid().ToString());
            var content = Utilities.GetRequestContent(jobUnit);
            
            await _client.PostAsync("api/Job/Schedule", content);

            var response = await _client.DeleteAsync($"api/Job/Unschedule/{jobUnit.Key}");

            response.EnsureSuccessStatusCode();

            var vm = await Utilities.GetResponseContent<Result>(response);

            Assert.True(vm.Success);
        }

        [Fact]
        public async Task Unschedule_WrongKeyReturnsNotFoundStatusCode()
        {
            var invalidId = "est";

            var response = await _client.DeleteAsync($"api/Job/Unschedule/{invalidId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Unschedule_EmptyKeyReturnsNotFoundStatusCode()
        {
            var response = await _client.DeleteAsync("api/Job/Unschedule");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}