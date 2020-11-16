using System;
using FluentValidation;

namespace Brutus.User.Api.Validators
{
    public class UserCreateValidator:AbstractValidator<Commands.V1.UserCreate>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserId).NotNull().Custom((userId, context) =>
            {
                if (userId == Guid.Empty) context.AddFailure("User id is mandatory field!");
            });
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}