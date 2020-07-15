using System.Collections.Generic;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class GetJobsList
    {
        private readonly IPersistStorage _storage;

        public GetJobsList(IPersistStorage storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<string>> GetJobs()
        {
            var jobsList = await _storage.GetJobsAsync();

            return jobsList;
        }
    }
}