using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests
{
    public class UserCreateTests
    {
        private readonly User _createdUser;
        private readonly Guid _userId = Guid.NewGuid();
        
        public UserCreateTests()
        {
            _createdUser = new User(_userId);
        }

        [Fact]
        public void ShouldBeCreatedWithCorrectData()
        {
            Assert.NotNull(_createdUser);
            Assert.Equal(_userId, _createdUser.Id);
        }
        
        [Fact]
        public void ShouldGenerateUserCreatedEvent()
        {
            var events = _createdUser.DequeueEvents();
            
            Assert.Single(events);
            Assert.Equal(typeof(Events.V1.UserCreated), events.Last().GetType());
        }

        [Fact]
        public void UserCreatedEventShouldContainCorrectData()
        {
            var @event = (Events.V1.UserCreated) _createdUser.DequeueEvents().Last();
            Assert.Equal(_createdUser.Id, @event.UserId);
        }
    }
}