using System;
using FileCopyJobService;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application
{
    public class TaskFactory
    {
        public IJobTask Create(string name, object dtoTaskParameters) => name switch
        {
            "FileCopy" => new FileCopyJob(dtoTaskParameters),
            _ => throw new Exception("Task with this alias was not found")
        };
    }
}