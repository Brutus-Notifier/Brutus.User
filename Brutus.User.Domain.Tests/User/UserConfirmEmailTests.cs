using System;
using System.Linq;
using Brutus.Core;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserConfirmEmailTests
    {
        private readonly Domain.User _user;

        public UserConfirmEmailTests()
        {
            _user = new Domain.User(Guid.NewGuid(), "Testing123!", "emaa@email.com");
        }

        [Fact]
        public void ShouldChangeAggregateDataOnSuccess()
        {
            _user.ConfirmEmail(_user.Email);
            Assert.Equal(Domain.User.UserStatus.Active, _user.Status);
        }
        
        [Fact]
        public void ShouldThrowAnExceptionIfEmailIsEmpty()
        {
            Action act = () => _user.ConfirmEmail("  ");
            DomainException exception = Assert.Throws<DomainException>(act);
            Assert.Equal("Email could not be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldGenerateUserEmailChangedEvent()
        {
            _user.ConfirmEmail(_user.Email);
            var @event = _user.DequeueEvents().Last();
            
            Assert.Equal(typeof(Events.V1.UserEmailConfirmed), @event.GetType());
        }
        
        [Fact]
        public void ShouldGenerateUserEmailChangedEventWithCorrectData()
        {
            _user.ConfirmEmail(_user.Email);
            var @event = (Events.V1.UserEmailConfirmed) _user.DequeueEvents().Last();
            
            Assert.Equal(_user.Email, @event.Email);
            Assert.Equal(_user.Id, @event.UserId);
        }
        
        [Fact]
        public void ShouldThrowAnExceptionIfEmailIsNotTheSameAsInUser()
        {
            const string anotherEmail = "another.email@email.com";
            DomainException exception = Assert.Throws<DomainException>(() => _user.ConfirmEmail(anotherEmail));
            Assert.Equal($"Incorrect Email. User doesn't have an email {anotherEmail}", exception.Message);
        }

        [Fact]
        public void ShouldThrowEnExceptionIfEmailHasBeenAlreadyConfirmed()
        {
            _user.ConfirmEmail(_user.Email);
            DomainException exception = Assert.Throws<DomainException>(() => _user.ConfirmEmail(_user.Email));
            Assert.Equal($"User already has confirmed {_user.Email} email", exception.Message);
        }
    }
}