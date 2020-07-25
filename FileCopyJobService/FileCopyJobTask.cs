#nullable enable
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileCopyJobService
{
    public class FileCopyJobTask : IJobTask
    {
        private ILogger _logger;

        public FileCopyJobTask(ILogger logger)
        {
            _logger = logger;
        }

        //TODO: clear this, and how to add logging?
        public Task Invoke(object? state)
        {
            try
            {
                if (state == null)
                {
                    _logger.LogError($"Job {nameof(FileCopyJobTask)} required parameters are missing");
                    return Task.CompletedTask;
                }

                var parameters = ParseParameters(state.ToString());

                File.Copy(parameters.From + @"\" + parameters.FileName, parameters.To + @"\" + parameters.FileName);

                Console.WriteLine(
                    $"File:{parameters.FileName} was copied From: {parameters.From}  To: {parameters.To}");

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError($"Job {nameof(FileCopyJobTask)} failed");
                return Task.CompletedTask;
            }
        }

        private FileCopyParameters ParseParameters(string? parameters)
        {
            var desParameters = JsonSerializer.Deserialize<FileCopyParameters>(parameters);

            if (!Directory.Exists(desParameters.From) || !Directory.Exists(desParameters.To))
            {
                _logger.LogError(
                    $"One of route parameter is incorrect. From:{desParameters.From} or To:{desParameters.To} - not exists.");
                throw new Exception(
                    $"One of route parameter is incorrect. From:{desParameters.From} or To:{desParameters.To} - not exists.");
            }

            if (!string.IsNullOrEmpty(desParameters.FileName)) return desParameters;

            if (!File.Exists(desParameters.From + @"\" + desParameters.FileName))
            {
                _logger.LogError(
                    $"File is not exist in this route: {desParameters.From + @"\" + desParameters.FileName}");
                throw new Exception(
                    $"File is not exist in this route: {desParameters.From + @"\" + desParameters.FileName}");
            }

            _logger.LogError($"File name is empty:{desParameters.FileName}");
            throw new Exception($"File name is empty:{desParameters.FileName}");
        }
    }
}