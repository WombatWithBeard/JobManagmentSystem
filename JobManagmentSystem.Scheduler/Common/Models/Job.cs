﻿using System;
using System.Text.Json.Serialization;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler.Common.Models
{
    public class Job
    {
        public Job()
        {
        }

        [JsonIgnore] public IJobTask Task { get; set; }
        public string Name { get; set; }
        public object TaskParameters { get; set; }
        public string Key { get; set; }
        public Schedule Schedule { get; set; }
        public bool Enabled { get; set; }

        public Job(IJobTask task, DateTime timeStart, double interval, int intervalType, string name,
            object taskParameters, string key = null, bool enabled = true)
        {
            Name = name;
            TaskParameters = taskParameters;
            Task = task;
            Schedule = new Schedule(timeStart, intervalType, interval);
            Key = key ?? Guid.NewGuid().ToString();
            Enabled = enabled;
        }
    }
}