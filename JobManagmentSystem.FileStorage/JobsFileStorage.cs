using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.FileStorage
{
    public class JobsFileStorage : IPersistStorage
    {
        private readonly ILogger<JobsFileStorage> _logger;
        private readonly string _fileName;
        private readonly string _path;

        public JobsFileStorage(ILogger<JobsFileStorage> logger, string fileName = @"\jobs.ndjson")
        {
            _logger = logger;
            _fileName = fileName;
            _path = Directory.GetCurrentDirectory() + _fileName;
        }

        public async Task<(bool success, string message)> SaveJobAsync(string jsonJob, string key)
        {
            try
            {
                if (File.Exists(_path))
                {
                    var jobs = await File.ReadAllLinesAsync(_path);

                    if (jobs.Any(j => j.Contains(key))) return (false, $"Key {key} already exists");
                }

                await File.AppendAllLinesAsync(Directory.GetCurrentDirectory() + _fileName,
                    new[] {jsonJob});

                return (true, $"Job {key} saved successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> DeleteJobAsync(string key)
        {
            try
            {
                if (!File.Exists(_path)) return (false, "File not exists");

                var jobs = await File.ReadAllLinesAsync(_path);

                if (!jobs.Any(j => j.Contains(key))) return (false, $"Key {key} not exists");

                var newJobs = jobs.Where(j => !j.Contains(key));

                await File.WriteAllLinesAsync(_path, newJobs);

                return (true, $"Key {key} successfully deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> DeleteAllJobAsync()
        {
            try
            {
                if (!File.Exists(_path)) return (true, "File not exists");

                await File.WriteAllLinesAsync(_path, new List<string>());

                return (true, "All jobs was deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message, string[] jobs)> GetJobsAsync()
        {
            try
            {
                if (!File.Exists(_path)) return (false, "File not exists", null);

                var result = await File.ReadAllLinesAsync(_path);

                return (true, "File read successfully", result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message, null);
            }
        }

        public async Task<(bool success, string message, string job)> GetJobAsync(string key)
        {
            try
            {
                if (!File.Exists(_path)) return (false, "File not exists", null);

                var jobs = await File.ReadAllLinesAsync(_path);

                if (jobs == null) return (false, "Jobs list was empty", null);

                var job = jobs.FirstOrDefault(j => j.Contains(key));

                if (job == null) return (false, "Jobs list was empty", null);

                return (true, "There is ur job, boy", job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message, null);
            }
        }
    }
}