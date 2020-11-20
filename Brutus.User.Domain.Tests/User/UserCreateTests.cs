using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserCreateTests
    {
        private readonly Domain.User _createdUser;
        private readonly (Guid Id, string Password, string Email) _userData;
        
        public UserCreateTests()
        {
            _userData = (Id: Guid.NewGuid(), Password: "Testing123!", Email: "test@email.com");
            _createdUser = new Domain.User(_userData.Id, _userData.Password, _userData.Email);
        }

        [Fact]
        public void ShouldBeCreatedWithCorrectData()
        {
            Assert.NotNull(_createdUser);
            Assert.Equal(_userData.Id, _createdUser.Id);
            Assert.Equal(_userData.Password, _createdUser.Password);
            Assert.Equal(_userData.Email, _createdUser.Email);
            Assert.Equal(Domain.User.UserStatus.Pending, _createdUser.Status);
            Assert.Null(_createdUser.FirstName);
            Assert.Null(_createdUser.LastName);
        }
        
        [Fact]
        public void ShouldGenerateUserCreatedEvent()
        {
            var events = _createdUser.DequeueEvents();
            
            Assert.Single(events);
            Assert.Equal(typeof(Events.V1.UserCreated), events.Last().GetType());
        }

        [Fact]
        public void UserCreatedEventShouldContainsCorrectData()
        {
            var @event = (Events.V1.UserCreated) _createdUser.DequeueEvents().Last();
            Assert.Equal(_createdUser.Id, @event.UserId);
            Assert.Equal(_userData.Password, @event.Password);
            Assert.Equal(_userData.Email, @event.Email);
        }
    }
}