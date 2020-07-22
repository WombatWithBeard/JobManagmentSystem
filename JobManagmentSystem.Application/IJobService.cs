using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;

namespace JobManagmentSystem.Application
{
    public interface IJobService
    {
        Task<Result> ScheduleJobAsync(JobDto dto);
        Task<Result> RescheduleJobAsync(JobDto dto);
        Task<Result> UncheduleJobAsync(string key);
        Task<Result<Job>> GetJobByIdAsync(string key);
        Task<Result<Job[]>> GetAllJobsAsync();
    }
}