namespace JobManagmentSystem.Scheduler.Common.Exception
{
    public class SaveJobException: System.Exception
    {
        public SaveJobException(string name, object key)
            : base($"Save job:\"{name}\" id:\"{key}\" was failed.")
        {
        }
    }
}