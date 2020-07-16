#nullable enable
using System;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace FileCopyJobService
{
    public class FileCopyJob : IJobTask
    {
        private readonly FileCopyParameters _parameters;

        public FileCopyJob(object parameters)
        {
            _parameters = JsonSerializer.Deserialize<FileCopyParameters>(parameters.ToString());
        }

        public Task Invoke(object? state)
        {
            // throw new Exception("Я пизданулось");
            Console.WriteLine($"Откуда: {_parameters.From}  |куда: в {_parameters.To}");
            return Task.CompletedTask;
        }
    }
}