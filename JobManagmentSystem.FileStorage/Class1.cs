using System;
using System.Collections.Generic;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.FileStorage
{
    public class Class1 : IPersistStorage
    {
        public void SaveJob(string job)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetJobs()
        {
            throw new NotImplementedException();
        }

        public string GetJob()
        {
            throw new NotImplementedException();
        }
    }
}