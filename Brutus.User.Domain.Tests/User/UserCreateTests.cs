using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserCreateTests
    {
        private readonly Domain.User _createdUser;
        private readonly (Guid Id, string FirstName, string LastName, string Email) _userData;
        
        public UserCreateTests()
        {
            _userData = (Id: Guid.NewGuid(), FirstName: "Test First Name", LastName: "Test Last Name", Email: "test@email.com");
            _createdUser = new Domain.User(_userData.Id, _userData.FirstName, _userData.LastName, _userData.Email);
        }

        [Fact]
        public void ShouldBeCreatedWithCorrectData()
        {
            Assert.NotNull(_createdUser);
            Assert.Equal(_userData.Id, _createdUser.Id);
            Assert.Equal(_userData.FirstName, _createdUser.FirstName);
            Assert.Equal(_userData.LastName, _createdUser.LastName);
            Assert.Equal(_userData.Email, _createdUser.Email);
            Assert.Equal(Domain.User.UserStatus.Awaiting, _createdUser.Status);
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
            Assert.Equal(_userData.FirstName, @event.FirstName);
            Assert.Equal(_userData.LastName, @event.LastName);
            Assert.Equal(_userData.Email, @event.Email);
        }
    }
}