namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IScheduler
    {
        (bool success, string message) DeleteJobById(string key);
        (bool success, string message) DeleteAllJobs();
        (bool success, string message) AddJob(Job job);
    }
}