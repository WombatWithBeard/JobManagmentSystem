namespace JobManagmentSystem.Scheduler.Common.Exception
{
    public class UnscheduleJobException : System.Exception
    {
        public UnscheduleJobException(string key)
            : base($"Unscheduling job id:\"{key}\" was failed.")
        {
        }
    }
}