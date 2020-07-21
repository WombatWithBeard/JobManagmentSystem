using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Models;

namespace JobManagmentSystem.Application
{
    public interface IJobService
    {
        Task<(bool success, string message)> ScheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> RescheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> UncheduleJobAsync(string key);
        Task<(bool success, string message, Job job)> GetJobByIdAsync(string key);
        Task<(bool success, string message, Job[] jobs)> GetAllJobsAsync();
    }
}