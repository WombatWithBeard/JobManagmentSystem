using System.Text.Json;
using FileCopyJobService;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application
{
    public class TaskFactory
    {
        public IJobTask Create(string name, object dtoTaskParameters) => name switch
        {
            "FileCopy" => new FileCopyJob(dtoTaskParameters),
            _ => new FileCopyJob(JsonSerializer.Serialize(new {From = "Default From", To = "Default To"}))
        };
    }
}