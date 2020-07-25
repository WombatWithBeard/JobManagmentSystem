using System.Net.Http;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
using JobManagmentSystem.WebApi;
using Scheduler.IntegrationTests.Common;
using Xunit;

namespace Scheduler.IntegrationTests.Controllers.Jobs
{
    public class GetAll : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private TestJobMaker _jobMaker;

        public GetAll(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _jobMaker = new TestJobMaker();
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessStatusCode()
        {
            var jobUnit = _jobMaker.CreateJobDto();
            var content = Utilities.GetRequestContent(jobUnit);
            
            await _client.PostAsync("api/Job/Schedule", content);
            await _client.PostAsync("api/Job/Schedule", content);

            var response = await _client.GetAsync("api/Job/GetAll");

            response.EnsureSuccessStatusCode();

            var vm = await Utilities.GetResponseContent<Result<Job[]>>(response);

            Assert.True(vm.Success);
            Assert.NotNull(vm.Value);
        }
    }
}