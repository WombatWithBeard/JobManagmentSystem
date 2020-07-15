﻿using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IPersistStorage
    {
        Task<(bool success, string message)> SaveJobAsync(Job job);
        Task<(bool success, string message)> DeleteJobAsync(string key);
        Task<(bool success, string message, string[] result)> GetJobsAsync();
        Task<(bool success, string message, string result)> GetJobAsync(string key);
    }
}