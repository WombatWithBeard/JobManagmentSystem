#nullable enable
using System;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace ConsoleWriterJobService
{
    public class ConsoleWriteJob : IJobTask
    {
        public Task Invoke(object? state)
        {
            try
            {
                if (state == null)
                {
                    Console.WriteLine($"Job {nameof(ConsoleWriteJob)} required parameters are missing");
                    return Task.CompletedTask;
                }

                var parameters = JsonSerializer.Deserialize<ConsoleWriteParameters>(state.ToString());

                Console.WriteLine(parameters.Message);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.CompletedTask;
            }
        }
    }
}