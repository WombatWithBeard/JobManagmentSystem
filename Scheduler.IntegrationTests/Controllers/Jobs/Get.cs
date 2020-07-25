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
            
            await _client.PostAsync("api/Job/Schedule", content);

            var response = await _client.GetAsync($"api/Job/Get/{jobUnit.Key}");

            response.EnsureSuccessStatusCode();

            var vm = await Utilities.GetResponseContent<Result<Job>>(response);

            Assert.True(vm.Success);
            Assert.Equal(jobUnit.Key, vm.Value.Key);
        }

        [Fact]
        public async Task Get_WrongKeyReturnsNotFoundStatusCode()
        {
            var response = await _client.GetAsync("api/Job/Get/test");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task Get_EmptyKeyReturnsNotFoundStatusCode()
        {
            var response = await _client.GetAsync("api/Job/Get");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}