using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class GetJob
    {
        private readonly IPersistStorage _storage;

        public GetJob(IPersistStorage storage)
        {
            _storage = storage;
        }

        public async Task<string> GetJobById(string key)
        {
            var job = await _storage.GetJobAsync(key);

            return job;
        }
    }
}