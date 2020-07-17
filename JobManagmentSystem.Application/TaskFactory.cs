using System;
using ConsoleWriterJobService;
using FileCopyJobService;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Application
{
    public class TaskFactory
    {
        public IJobTask Create(string name) => name switch
        {
            TaskNameConstants.FileCopyJob => new FileCopyJob(),
            TaskNameConstants.ConsoleWriteJob => new ConsoleWriteJob(),
            _ => throw new Exception("Task with this alias was not found")
        };
    }

    public class TaskNameConstants
    {
        public const string FileCopyJob = "FileCopy";
        public const string ConsoleWriteJob = "ConsoleWrite";
    }
}