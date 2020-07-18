using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IPersistStorage
    {
        Task<(bool success, string message)> SaveJobAsync(string jsonJob, string key);
        Task<(bool success, string message)> DeleteJobAsync(string key);
        Task<(bool success, string message, string[] jobs)> GetJobsAsync();
        Task<(bool success, string message, string job)> GetJobAsync(string key);
    }
}