using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
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
            DirectoryCheck();
        }

        private void DirectoryCheck()
        {
            try
            {
                if (!File.Exists(_path)) File.Create(_path);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result> SaveJobAsync(string jsonJob, string key)
        {
            try
            {
                var jobs = await File.ReadAllLinesAsync(_path);

                if (jobs.Any(j => j.Contains(key))) return Result.Fail($"Key {key} already exists");

                await File.AppendAllLinesAsync(Directory.GetCurrentDirectory() + _fileName,
                    new[] {jsonJob});

                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result> DeleteJobAsync(string key)
        {
            try
            {
                var jobs = await File.ReadAllLinesAsync(_path);

                if (jobs == null) return Result.Ok("Storage is empty");

                if (!jobs.Any(j => j.Contains(key))) return Result.Fail($"Key {key} not exists");

                var newJobs = jobs.Where(j => !j.Contains(key));

                await File.WriteAllLinesAsync(_path, newJobs);

                return Result.Ok($"Key {key} successfully deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result<Job[]>> GetJobsAsync()
        {
            try
            {
                var stringsJobs = await File.ReadAllLinesAsync(_path);

                if (stringsJobs == null) 
                    return Result.Fail<Job[]>("Storage is empty");

                var jobs = stringsJobs.Select(j => JsonSerializer.Deserialize<Job>(j)).ToArray();

                return Result.Ok(jobs);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail<Job[]>(e.Message);
            }
        }

        public async Task<Result<Job>> GetJobAsync(string key)
        {
            try
            {
                var jobs = await File.ReadAllLinesAsync(_path);

                if (jobs == null) return Result.Fail<Job>("Jobs list was empty");

                var job = JsonSerializer.Deserialize<Job>(jobs.FirstOrDefault(j => j.Contains(key)));

                return job == null
                    ? Result.Fail<Job>("Jobs list was empty")
                    : Result.Ok(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail<Job>(e.Message);
            }
        }
    }
}