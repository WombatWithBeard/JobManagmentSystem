using System;

namespace JobManagmentSystem.Application.Common.Exceptions
{
    public class WrongKeyBadRequestException : Exception
    {
        public WrongKeyBadRequestException(string key)
            : base($"Key:\"{key}\" is incorrect.")
        {
        }
    }
}