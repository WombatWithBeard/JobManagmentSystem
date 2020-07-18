namespace JobManagmentSystem.Scheduler.Common.Exception
{
    public class DeleteJobException : System.Exception
    {
        public DeleteJobException(string key)
            : base($"Deleting job id:\"{key}\" was failed.")
        {
        }
    }
}