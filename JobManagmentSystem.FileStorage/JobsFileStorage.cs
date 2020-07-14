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
        public async Task SaveJobAsync(Job job)
        {
            var jobs = await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson");

            if (jobs.Any(j => j.Contains(job.Key)))
                throw new Exception($"Key: {job.Key} already exists");

            await File.AppendAllTextAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson",
                JsonSerializer.Serialize(job));
        }

        public async Task DeleteJobAsync(string key)
        {
            var jobs = await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson");

            if (!jobs.Any(j => j.Contains(key)))
                throw new Exception($"Key: {key} not exists");
            
            var newJobs = jobs.Where(j => !j.Contains(key));

            await File.WriteAllTextAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson",
                JsonSerializer.Serialize(newJobs));
        }

        public async Task<IEnumerable<string>> GetJobsAsync()
        {
            return await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson");
        }

        public async Task<string> GetJobAsync(string key)
        {
            var jobs = await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + @"\jobs.ndjson");

            try
            {
                return jobs.FirstOrDefault(j => j.Contains(key));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}