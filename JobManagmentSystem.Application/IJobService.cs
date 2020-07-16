using System.Collections.Generic;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Models;

namespace JobManagmentSystem.Application
{
    public interface IJobService
    {
        Task<(bool success, string message)> ScheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> RescheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> DeleteJobAsync(string key);
        Task<(bool success, string message, Job job)> GetScheduledJobByIdAsync(string key);
        Task<(bool success, string message, List<Job> jobs)> GetAllSchedulerJobsAsync();
    }
}