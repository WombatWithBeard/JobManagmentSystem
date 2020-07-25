using ConsoleWriterJobService;
using FileCopyJobService;
using JobManagmentSystem.Application.Common.Exceptions;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Application
{
    public class TaskFactory
    {
        private readonly ILogger _logger;

        public TaskFactory(ILogger<TaskFactory> logger)
        {
            _logger = logger;
        }

        public IJobTask Create(string name) => name switch
        {
            TaskNameConstants.FileCopyJob => new FileCopyJobTask(_logger),
            TaskNameConstants.ConsoleWriteJob => new ConsoleWriteJobTask(_logger),
            _ => throw new WrongTaskNameBadRequestException(name)
        };
    }

    public class TaskNameConstants
    {
        public const string FileCopyJob = "FileCopy";
        public const string ConsoleWriteJob = "ConsoleWrite";
    }
}