using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class DeleteJob
    {
        private readonly IScheduler _scheduler;
        private readonly IPersistStorage _storage;

        public DeleteJob(IScheduler scheduler, IPersistStorage storage)
        {
            _scheduler = scheduler;
            _storage = storage;
        }

        public async Task<bool> Delete(string key)
        {
            var isDeleted = _scheduler.DeleteJobById(key);

            if (isDeleted) await _storage.DeleteJobAsync(key);

            return isDeleted;
        }
    }
}