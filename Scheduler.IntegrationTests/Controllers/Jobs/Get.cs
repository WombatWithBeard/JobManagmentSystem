using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
using JobManagmentSystem.WebApi;
using Scheduler.IntegrationTests.Common;
using Xunit;

namespace Scheduler.IntegrationTests.Controllers.Jobs
{
    public class Get : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        private TestJobMaker _jobMaker;

        public Get(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jobMaker = new TestJobMaker();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        { 
            var jobUnit = _jobMaker.CreateJobDto(Guid.NewGuid().ToString());
            var content = Utilities.GetRequestContent(jobUnit);

            await _client.PostAsync("api/Job/Create", content);
            
            var response = await _client.GetAsync($"api/Job/Get/{jobUnit.Key}");

            response.EnsureSuccessStatusCode();

            var vm = await Utilities.GetResponseContent<Result>(response);

            Assert.True(vm.Success);
        }

        [Fact]
        public async Task Get_ReturnsNotFoundStatusCode()
        {
            var jobUnit = new object();
            var content = Utilities.GetRequestContent(jobUnit);

            var response = await _client.GetAsync("api/Job/Get");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}