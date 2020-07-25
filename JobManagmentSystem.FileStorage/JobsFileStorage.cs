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
        private readonly string _path;
        private static readonly object Locker = new object();

        public JobsFileStorage(ILogger<JobsFileStorage> logger, IOptions<FileStorage> options)
        {
            _logger = logger;
            _path = Directory.GetCurrentDirectory() + @"\" + options.Value.StoragePath;
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
                var jobs = await File.ReadAllLinesAsync(_path);

                if (!IsNullOrEmpty(jobs))
                    if (jobs.Any(j => j.Contains(job.Key)))
                        return Result.Fail(FileStorageConsts.KeyAlreadyExists);

                await File.AppendAllLinesAsync(_path, new[] {JsonSerializer.Serialize(job)});

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

                if (IsNullOrEmpty(jobs)) return Result.Fail(FileStorageConsts.StorageIsEmpty);

                if (!jobs.Any(j => j.Contains(key))) return Result.Fail(FileStorageConsts.KeyNotExists);

                var newJobs = jobs.Where(j => !j.Contains(key));

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
                var strJobs = File.ReadAllLinesAsync(_path).Result;

                if (IsNullOrEmpty(strJobs)) return Result.Fail<Job[]>(FileStorageConsts.StorageIsEmpty);

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
                var jobs = File.ReadAllLinesAsync(_path).Result;

                if (IsNullOrEmpty(jobs)) return Result.Fail<Job>(FileStorageConsts.JobsListWasEmpty);

                var job = JsonSerializer.Deserialize<Job>(jobs.FirstOrDefault(j => j.Contains(key)));

                if (job == null) Result.Fail<Job>(FileStorageConsts.KeyNotExists);

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
    }

    public class FileStorageConsts
    {
        public const string JobsListWasEmpty = "Jobs list was empty";
        public const string KeyNotExists = "Key not exists";
        public const string StorageIsEmpty = "Storage is empty";
        public const string KeyAlreadyExists = "Key already exists";
    }
}