using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;

namespace JobManagmentSystem.FileStorage
{
    public class JobsFileStorage : IPersistStorage
    {
        private readonly string _fileName;
        private readonly string _path;

        public JobsFileStorage(string fileName = @"\jobs.ndjson")
        {
            _fileName = fileName;
            _path = Directory.GetCurrentDirectory() + _fileName;
        }

        //TODO: a lot of ...
        //TODO: add logging

        public async Task<(bool success, string message)> SaveJobAsync(string jsonJob, string key)
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

        public async Task<(bool success, string message)> DeleteJobAsync(string key)
        {
            if (!File.Exists(_path)) return (false, "File not exists");

            var jobs = await File.ReadAllLinesAsync(_path);

            if (!jobs.Any(j => j.Contains(key))) return (false, $"Key {key} not exists");

            var newJobs = jobs.Where(j => !j.Contains(key));

            await File.WriteAllLinesAsync(_path, newJobs);

            return (true, $"Key {key} successfully deleted");
        }

        public async Task<(bool success, string message)> DeleteAllJobAsync()
        {
            if (!File.Exists(_path)) return (true, "File not exists");

            await File.WriteAllLinesAsync(_path, new List<string>());

            return (true, "All jobs was deleted");
        }

        public async Task<(bool success, string message, string[] result)> GetJobsAsync()
        {
            if (!File.Exists(_path)) return (false, "File not exists", null);

            var result = await File.ReadAllLinesAsync(_path);

            return (true, "File read successfully", result);
        }

        public async Task<(bool success, string message, string result)> GetJobAsync(string key)
        {
            if (!File.Exists(_path)) return (false, "File not exists", null);

            var jobs = await File.ReadAllLinesAsync(_path);

            if (jobs == null) return (false, "Jobs list was empty", null);

            var job = jobs.FirstOrDefault(j => j.Contains(key));

            if (job == null) return (false, "Jobs list was empty", null);

            return (true, "There is ur job, boy", job);
        }
    }
}