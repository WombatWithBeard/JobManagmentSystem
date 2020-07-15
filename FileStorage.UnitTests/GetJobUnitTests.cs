using JobManagmentSystem.FileStorage;

namespace FileStorage.UnitTests
{
    public class GetJobUnitTests
    {
        private readonly JobsFileStorage _jobFileStorage;
        private readonly BaseFileCreator _baseFile;

        public GetJobUnitTests()
        {
            _baseFile = new BaseFileCreator();
            _jobFileStorage = new JobsFileStorage(_baseFile.TestFileName);
        }
    }
}