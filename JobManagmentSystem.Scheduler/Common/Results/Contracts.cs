using System;

namespace JobManagmentSystem.Scheduler.Common.Results
{
    public class Contracts
    {
        public static void Require(bool success)
        {
            if (!success)
            {
                throw new Exception();
            }
        }
    }
}