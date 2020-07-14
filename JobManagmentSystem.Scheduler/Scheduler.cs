using System.Threading;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class Scheduler
    {
        public Timer Schedule(IJobTask task, Schedule schedule)
        {
            var timer = new Timer(o => task.Invoke(o), task, schedule.WhenStart, schedule.Period);
            return timer;
        }
    }
}