using System;

namespace HondataDotNet.Datalog.Core
{
    [Serializable]
    public class InvalidDatalogFileException : Exception
    {
        public InvalidDatalogFileException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
