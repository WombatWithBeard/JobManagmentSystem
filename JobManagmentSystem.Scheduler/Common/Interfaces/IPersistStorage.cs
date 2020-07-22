using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IPersistStorage
    {
        Task<Result> SaveJobAsync(string jsonJob, string key);
        Task<Result> DeleteJobAsync(string key);
        Task<Result<Job[]>> GetJobsAsync();
        Task<Result<Job>> GetJobAsync(string key);
    }
}