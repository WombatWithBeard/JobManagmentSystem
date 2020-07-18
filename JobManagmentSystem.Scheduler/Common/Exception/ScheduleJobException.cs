namespace JobManagmentSystem.Scheduler.Common.Exception
{
    public class ScheduleJobException : System.Exception
    {
        public ScheduleJobException(string name, object key)
            : base($"Scheduling job:\"{name}\" id:\"{key}\" was failed.")
        {
        }
    }
}