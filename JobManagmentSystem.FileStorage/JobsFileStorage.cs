using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                if (File.Exists(_path)) return;
                using (File.Create(_path))
                {
                    _logger.LogInformation($"Jobs storage was created in path: {_path}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result> SaveJobAsync(Job job)
        {
            try
            {
                if (IsFileLocked()) await Task.Delay(2000);
                var jobs = await File.ReadAllLinesAsync(_path);

                if (!IsNullOrEmpty(jobs))
                    if (jobs.Any(j => j.Contains(job.Key)))
                        return Result.Fail($"Key {job.Key} already exists");

                if (IsFileLocked()) await Task.Delay(2000);
                await File.AppendAllLinesAsync(Directory.GetCurrentDirectory() + _fileName,
                    new[] {JsonSerializer.Serialize(job)});

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
                if (IsFileLocked()) await Task.Delay(2000);
                var jobs = await File.ReadAllLinesAsync(_path);

                if (IsNullOrEmpty(jobs)) return Result.Ok("Storage is empty");

                if (!jobs.Any(j => j.Contains(key))) return Result.Fail($"Key {key} not exists");

                var newJobs = jobs.Where(j => !j.Contains(key));

                if (IsFileLocked()) await Task.Delay(2000);
                await File.WriteAllLinesAsync(_path, newJobs);

                return Result.Ok();
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
                if (IsFileLocked()) await Task.Delay(2000);
                var strJobs = await File.ReadAllLinesAsync(_path);

                if (IsNullOrEmpty(strJobs)) return Result.Fail<Job[]>("Storage is empty");

                var jobs = strJobs.Select(j => JsonSerializer.Deserialize<Job>(j)).ToArray();

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
                if (IsFileLocked()) await Task.Delay(2000);
                var jobs = await File.ReadAllLinesAsync(_path);

                if (IsNullOrEmpty(jobs)) return Result.Fail<Job>("Jobs list was empty");

                var job = JsonSerializer.Deserialize<Job>(jobs.FirstOrDefault(j => j.Contains(key)));

                if (job == null) Result.Fail<Job>("Jobs list was empty");

                return Result.Ok(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail<Job>(e.Message);
            }
        }

        private bool IsNullOrEmpty<T>(IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        
        protected virtual bool IsFileLocked()
        {
            try
            {
                using var stream = File.Open(_path,FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                return true;
            }
            
            return false;
        }
    }
}