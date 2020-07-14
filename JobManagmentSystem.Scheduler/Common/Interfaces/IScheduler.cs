using System.Threading;
using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IScheduler
    {
        bool DeleteJobById(string key);
        bool DeleteAllJobs();
        bool AddJob(Job job);
    }
}