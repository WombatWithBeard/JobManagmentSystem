namespace JobManagmentSystem.Scheduler.Common.Exception
{
    public class NotFoundException : System.Exception
    {
        public NotFoundException(string key)
            : base($"Job: \"{key}\" was not found.")
        {
        }
    }
}