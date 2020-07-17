#nullable enable
using System;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileCopyJobService
{
    public class FileCopyJob : IJobTask
    {
        //TODO: clear this, and how to add logging?
        public Task Invoke(object? state)
        {
            try
            {
                if (state == null)
                {
                    // _logger.LogError($"Job {nameof(FileCopyJob)} required parameters are missing");
                    return Task.CompletedTask;
                }

                var parameters = JsonSerializer.Deserialize<FileCopyParameters>(state.ToString());

                Console.WriteLine($"Откуда: {parameters.From}  |куда: в {parameters.To}");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                // _logger.LogError($"Job {nameof(FileCopyJob)} failed");
                return Task.CompletedTask;
            }
        }
    }
}