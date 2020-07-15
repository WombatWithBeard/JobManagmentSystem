using System.Linq;
using System.Threading.Tasks;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Enums;
using Xunit;

namespace FileStorage.UnitTests
{
    public class SaveJobUnitTests
    {
        private readonly JobsFileStorage _jobFileStorage;
        private readonly BaseFileCreator _baseFile;

        public SaveJobUnitTests()
        {
            _baseFile = new BaseFileCreator();
            _jobFileStorage = new JobsFileStorage(_baseFile.TestFileName);
        }

        [Fact]
        public async Task SaveJob_CorrectResult()
        {
            //Arrange
            var testJob = new Job(null, 10, (int) IntervalsEnum.Daily);

            //Act
            await _jobFileStorage.SaveJobAsync(testJob);
            var jobs = await _baseFile.GetTestFileData();

            //Assert
            Assert.Contains(jobs, s => s.Contains(testJob.Key));
        }
    }
}