using System;

namespace Brutus.Core
{
    [Serializable]
    public class DomainException: Exception
    {
        public string Domain { get; private set; }
        public DomainException(string domain, string message) : base(message)
        {
            Domain = domain;
        }

        public DomainException(string domain, string message, Exception inner) : base(message, inner)
        {
            Domain = domain;
        }
    }
}