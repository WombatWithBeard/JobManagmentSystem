#nullable enable
using System;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConsoleWriterJobService
{
    public class ConsoleWriteJobTask : IJobTask
    {
        private readonly ILogger _logger;

        public ConsoleWriteJobTask(ILogger logger)
        {
            _logger = logger;
        }

        public Task Invoke(object? state)
        {
            try
            {
                if (state == null)
                {
                    _logger.LogError($"Job {nameof(ConsoleWriteJobTask)} required parameters are missing");
                    return Task.CompletedTask;
                }

                var parameters = JsonSerializer.Deserialize<ConsoleWriteParameters>(state.ToString());

                Console.WriteLine(parameters.Message);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Task.CompletedTask;
            }
        }
    }
}