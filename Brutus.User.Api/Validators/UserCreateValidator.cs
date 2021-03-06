using Brutus.Core;
using FluentValidation;

namespace Brutus.User.Api.Validators
{
    public class UserCreateValidator:AbstractValidator<Commands.V1.UserCreate>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).Matches(Constants.PasswordTemplate);
        }
    }
}