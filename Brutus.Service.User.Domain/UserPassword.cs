using System;
using System.Text.RegularExpressions;
using Brutus.Framework;

namespace Brutus.Service.User.Domain
{
    public class UserPassword : Value<UserPassword>
    {
        private const string PasswordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        public string HashedValue { get; private set; }

        internal UserPassword(string value) => HashedValue = value;
        
        public static UserPassword Parse(string value, IPasswordObfuscator obfuscator)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "User password could not be empty");
            
            if (!Regex.IsMatch(value, PasswordPattern))
                throw new ArgumentException("User password doest not meet the security criteria");
            
            return new UserPassword(obfuscator.Obfuscate(value));
        }
        
        public static implicit operator string(UserPassword self) => self.HashedValue;
    }
}