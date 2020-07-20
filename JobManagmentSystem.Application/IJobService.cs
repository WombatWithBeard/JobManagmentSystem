using System.Threading.Tasks;

namespace JobManagmentSystem.Application
{
    public interface IJobService
    {
        Task<(bool success, string message)> ScheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> RescheduleJobAsync(JobDto dto);
        Task<(bool success, string message)> UncheduleJobAsync(string key);
        Task<(bool success, string message, string job)> GetJobByIdAsync(string key);
        Task<(bool success, string message, string[] jobs)> GetAllJobsAsync();
    }
}