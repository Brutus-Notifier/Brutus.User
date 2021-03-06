using System;
using System.Linq;
using Brutus.Core;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserChangeEmailTests
    {
        private readonly Domain.User _user;
        private readonly (Guid Id, string Password, string Email) _userData;
        private readonly string _userEmail = "newuser@email.com";

        public UserChangeEmailTests()
        {
            _userData = (Id: Guid.NewGuid(), Password: "Testing123!", Email: "initial_user@email.com");
            _user = new Domain.User(_userData.Id, _userData.Password, _userData.Email);
        }

        [Fact]
        public void ShouldCreateUserEmailChangedEventOnSucceed()
        {
            _user.ChangeEmail(_userEmail);
            
            var events = _user.DequeueEvents();
            Assert.Equal(typeof(Events.V1.UserEmailChanged), events.Last().GetType());
        }
        
        [Fact]
        public void ShouldHaveCorrectDataInCreateUserEmailChangedEventOnSucceed()
        {
            _user.ChangeEmail(_userEmail);

            var receivedEvent = _user.DequeueEvents().Last() as Events.V1.UserEmailChanged;
            
            Assert.Equal(_userData.Id, receivedEvent.UserId);
            Assert.Equal(_userEmail, receivedEvent.Email);
        }
        
        [Fact]
        public void ShouldChangeAggregateStateOnSucceed()
        {
            _user.ChangeEmail(_userEmail);
            Assert.Equal(_userEmail, _user.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ShouldThrowExceptionIfEmailIsEmpty(string userEmail)
        {
            DomainException exception = Assert.Throws<DomainException>(() => _user.ChangeEmail(userEmail));
            Assert.Equal("Email could not be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldThrowExceptionIfEmailLongerThen50Chars()
        {
            var longEmail = "veryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy.long@email.com";
            DomainException exception = Assert.Throws<DomainException>(() => _user.ChangeEmail(longEmail));
            Assert.Equal("Email could not be longer than 50 characters", exception.Message);
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("#@%^%#$@#$@#.com")]
        [InlineData("@example.com")]
        [InlineData("Joe Smith <email@example.com>")]
        [InlineData("email.example.com")]
        [InlineData("email@example@example.com")]
        [InlineData(".email@example.com")]
        [InlineData("email.@example.com")]
        [InlineData("email..email@example.com")]
        [InlineData("あいうえお@example.com")]
        [InlineData("email@example.com (Joe Smith)")]
        [InlineData("email@example")]
        [InlineData("email@-example.com")]
        [InlineData("email@example..com")]
        [InlineData("Abc..123@example.com")]
        public void ShouldThrowExceptionIfEmailIsInvalid(string email)
        {
            DomainException exception = Assert.Throws<DomainException>(() => _user.ChangeEmail(email));
            Assert.Equal($"Email {email} is invalid", exception.Message);
        }
        
        [Fact]
        public void ShouldTrimStartAndEndSpacesInEmail()
        {
            var spacedEmail = $"   {_userEmail}   ";
            _user.ChangeEmail(spacedEmail);
            Assert.Equal(_userEmail, _user.Email);
        }
    }
}