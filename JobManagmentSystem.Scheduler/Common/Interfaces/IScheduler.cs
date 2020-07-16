using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Models;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<(bool success, string message)> ScheduleJob(Job job);
        Task<(bool success, string message)> RescheduleJob(Job job);
        Task<(bool success, string message)> UnscheduleJobById(string key);
        Task<(bool success, string message)> UnscheduleAllJobs();
    }
}