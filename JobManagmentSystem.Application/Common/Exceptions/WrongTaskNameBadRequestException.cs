using System;

namespace JobManagmentSystem.Application.Common.Exceptions
{
    public class WrongTaskNameBadRequestException : Exception
    {
        public WrongTaskNameBadRequestException(string name)
            : base($"Task with name:\"{name}\" was not found.")
        {
        }
    }
}