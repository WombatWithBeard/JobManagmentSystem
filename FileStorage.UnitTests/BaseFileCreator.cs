using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common;
using JobManagmentSystem.Scheduler.Common.Enums;

namespace FileStorage.UnitTests
{
    public class BaseFileCreator
    {
        public string TestFileName = @"\testsjobs.ndjson";
        public static string Path;

        public BaseFileCreator()
        {
            Path = Directory.GetCurrentDirectory() + TestFileName;
        }

        public async Task CreateBaseFile()
        {
            var jobs = CreateFewJobs();

            await File.WriteAllTextAsync(Path,
                JsonSerializer.Serialize(jobs));
        }

        public async Task<IEnumerable<string>> GetTestFileData()
        {
            return await File.ReadAllLinesAsync(Path);
        }

        private IEnumerable<Job> CreateFewJobs() => new List<Job>
        {
            new Job(null, 10, (int) IntervalsEnum.Daily),
            new Job(null, 12, (int) IntervalsEnum.Monthly),
            new Job(null, 20, (int) IntervalsEnum.Hours)
        };
    }
}