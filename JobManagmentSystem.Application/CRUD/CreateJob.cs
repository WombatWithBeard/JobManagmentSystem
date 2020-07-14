using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application.CRUD
{
    public class CreateJob
    {
        private readonly IScheduler _scheduler;

        public CreateJob(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public bool Create(JobDto dto)
        {
            var task = new TaskFactory().Create(dto.TaskName, dto.TaskParameters);

            var job = new Job(task, dto.TimeStart, (int) dto.Interval); //TODO: interval

            var isScheduled = _scheduler.AddJob(job);

            return isScheduled;
        }
    }
}