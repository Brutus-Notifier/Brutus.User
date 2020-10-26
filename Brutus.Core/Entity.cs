using System;

namespace Brutus.Core
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        
        protected void CheckNullOrEmpty(string paramValue, string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramValue))
                throw new ArgumentException($"{paramName} could not be null or empty");
        }
        
        protected void CheckMaxLength(int maxLength, string paramValue, string paramName)
        {
            if (paramValue.Length > maxLength)
                throw new ArgumentException($"{paramName} could not be longer than {maxLength} characters");
        }
    }
}