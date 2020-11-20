using System;
using System.Text.RegularExpressions;

namespace Brutus.Core
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        
        protected void CheckNullOrEmpty(string paramValue, string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramValue))
                throw new DomainException(GetType().Name, $"{paramName} could not be null or empty");
        }
        
        protected void CheckMaxLength(int maxLength, string paramValue, string paramName)
        {
            if (paramValue.Length > maxLength)
                throw new DomainException(GetType().Name, $"{paramName} could not be longer than {maxLength} characters");
        }

        protected void CheckIsMatch(string template, string paramValue, string paramName)
        {
            if(!Regex.IsMatch(paramValue, template, RegexOptions.IgnoreCase))
                throw new DomainException(GetType().Name, $"{paramName} {paramValue} is invalid");
        }
    }
}