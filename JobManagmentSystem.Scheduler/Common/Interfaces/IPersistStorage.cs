using System.Collections.Generic;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IPersistStorage
    {
        void SaveJob(string job);
        ICollection<string> GetJobs();
        string GetJob();
    }
}