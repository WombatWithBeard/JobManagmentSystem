using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IPersistStorage
    {
        Task SaveJobAsync(Job job);
        Task DeleteJobAsync(string key);
        Task<IEnumerable<string>> GetJobsAsync();
        Task<string> GetJobAsync(string key);
    }
}