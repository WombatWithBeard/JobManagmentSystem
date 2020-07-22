using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<Result> ScheduleJobAsync(Job job);
        Task<Result> UnscheduleJobAsync(string key);
        Task<Result> RescheduleJobAsync(Job job);
        Task<Result<Job>> GetJob(string key);
        Task<Result<Job[]>> GetJobs();
    }
}