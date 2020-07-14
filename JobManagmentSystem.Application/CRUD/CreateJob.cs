using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class CreateJob
    {
        private readonly IScheduler _scheduler;
        private readonly IPersistStorage _storage;

        public CreateJob(IScheduler scheduler, IPersistStorage storage)
        {
            _scheduler = scheduler;
            _storage = storage;
        }

        public async Task<bool> Create(JobDto dto)
        {
            var task = new TaskFactory().Create(dto.TaskName, dto.TaskParameters);

            var job = new Job(task, dto.TimeStart, (int) dto.Interval); //TODO: interval

            var isScheduled = _scheduler.AddJob(job);

            if (isScheduled) await _storage.SaveJobAsync(job);

            return isScheduled;
        }
    }
}