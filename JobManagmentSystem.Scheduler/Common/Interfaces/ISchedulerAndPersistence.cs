using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface ISchedulerAndPersistence
    {
        Task<(bool success, string message)> CreateJobAsync(Job job);
        Task<(bool success, string message)> DeleteJobAsync(string key);
        Task<(bool success, string message)> ReScheduleJobAsync(Job job);
        Task<(bool success, string message, string job)> GetJobAsync(string key);
        Task<(bool success, string message, string[] jobs)> GetJobsListAsync();
    }
}