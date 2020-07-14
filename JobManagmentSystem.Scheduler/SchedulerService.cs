using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class SchedulerService : IScheduler
    {
        private readonly Dictionary<string, Timer> _timers;
        // private static SchedulerService _instance;

        public SchedulerService()
        {
            _timers = new Dictionary<string, Timer>();
        }

        // public static SchedulerService Instance => _instance ??= new SchedulerService();
        public bool AddJob(Job job)
        {
            try
            {
                _timers.Add(job.Key, job.Timer);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool DeleteJobById(string key)
        {
            try
            {
                _timers.Remove(key);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool DeleteAllJobs()
        {
            try
            {
                _timers.Clear();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}