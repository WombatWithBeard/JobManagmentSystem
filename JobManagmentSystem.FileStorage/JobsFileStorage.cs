using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.FileStorage
{
    public class JobsFileStorage : IPersistStorage
    {
        private readonly string _fileName;
        private string _path;

        public JobsFileStorage(string fileName = @"\jobs.ndjson")
        {
            _fileName = fileName;
            _path = Directory.GetCurrentDirectory() + _fileName;
        }

        public async Task SaveJobAsync(Job job)
        {
            if (File.Exists(_path))
            {
                var jobs = await File.ReadAllLinesAsync(_path);

                if (jobs.Any(j => j.Contains(job.Key)))
                    throw new Exception($"Key: {job.Key} already exists");
            }

            await File.AppendAllTextAsync(Directory.GetCurrentDirectory() + _fileName, JsonSerializer.Serialize(job));
        }

        public async Task DeleteJobAsync(string key)
        {
            if (File.Exists(_path))
            {
                var jobs = await File.ReadAllLinesAsync(_path);

                if (!jobs.Any(j => j.Contains(key)))
                    throw new Exception($"Key: {key} not exists");

                var newJobs = jobs.Where(j => !j.Contains(key));

                await File.WriteAllTextAsync(_path, JsonSerializer.Serialize(newJobs));
            }
        }

        public async Task<IEnumerable<string>> GetJobsAsync()
        {
            if (File.Exists(_path))
                return await File.ReadAllLinesAsync(_path);

            return new List<string>();
        }

        public async Task<string> GetJobAsync(string key)
        {
            if (!File.Exists(_path)) return "";

            var jobs = await File.ReadAllLinesAsync(_path);

            return jobs.FirstOrDefault(j => j.Contains(key));
        }
    }
}