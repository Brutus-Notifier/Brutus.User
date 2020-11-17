using System;

namespace Brutus.User.Exceptions
{
    public class DataValidationException : Exception
    {
        public string Domain { get; }
        public string Field { get; }

        public DataValidationException(string domain, string field, string message) : base(message)
        {
            Domain = domain;
            Field = field;
        }

        public DataValidationException(string domain, string field, string message, Exception inner) 
            : base(message, inner)
        {
            Domain = domain;
            Field = field;
        }
    }
}