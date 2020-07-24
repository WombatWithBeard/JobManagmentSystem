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
            var response =
                await _client.DeleteAsync($"api/Job/Delete/test0");

            var vm = await Utilities.GetResponseContent<Result>(response);
            
            response.EnsureSuccessStatusCode();
        }

        // [Fact]
        // public async Task Unschedule_ReturnsNotFoundStatusCode()
        // {
        //     var invalidId = "";
        //
        //     var response =
        //         await _client.DeleteAsync(UriForTests.DeleteUri(ControllerNames.AuthenticationOptions, invalidId));
        //
        //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        // }
    }
}