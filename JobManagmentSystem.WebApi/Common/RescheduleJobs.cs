using System.Linq;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;

namespace JobManagmentSystem.WebApi.Common
{
    public class RescheduleJobs
    {
        public static void Reschedule(IScheduler scheduler, IPersistStorage storage)
        {
            var jobs = storage.GetJobsAsync().Result;
            jobs.OnSuccess(() => jobs.Value.ToList().ForEach(async x => await scheduler.ScheduleJobAsync(x)));
        }
    }
}