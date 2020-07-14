using System;
using System.Net.Mail;
using Brutus.Framework;

namespace Brutus.Service.User.Domain
{
    public class UserEmail : Value<UserEmail>
    {
        public string Value { get; private set; }

        public UserEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "User email could not be empty");
            
            try
            {
                MailAddress m = new MailAddress(value);
                Value = value;
            }
            catch (FormatException)
            {
                throw new ArgumentException("User email has incorrect format", nameof(value));
            }
        }

        public static implicit operator string(UserEmail self) => self.Value;
    }
}