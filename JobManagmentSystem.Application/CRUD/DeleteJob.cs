using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class DeleteJob
    {
        private readonly IScheduler _scheduler;

        public DeleteJob(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Delete(string key)
        {
            var isDeleted = _scheduler.DeleteJobById(key);
        }
    }
}