using System;

namespace HondataDotNet.Datalog.Core
{
    [Serializable]
    public sealed class InvalidDatalogFormatException : Exception
    {
        public InvalidDatalogFormatException(string? message) : base(message)
        {
        }
    }
}