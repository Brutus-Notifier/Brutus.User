using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests
{
    public class UserChangeNameTests
    {
        private readonly User _user;
        private readonly Guid _userId = new Guid();
        private readonly string _userName = "Test Name";

        public UserChangeNameTests()
        {
            _user = new User(_userId);
            _user.ChangeName(_userName);
        }

        [Fact]
        public void ShouldChangeAggregateData()
        {
            Assert.Equal(_userName, _user.Name);
        }

        [Fact]
        public void ShouldCreateUserNameChangedEvent()
        {
            var events = _user.DequeueEvents();
            Assert.Equal(2, events.Count);
            Assert.Equal(typeof(Events.V1.UserNameChanged), events.Last().GetType());
        }
        
        [Fact]
        public void UserNameChangedEventShouldContainCorrectData()
        {
            var @event = (Events.V1.UserNameChanged)_user.DequeueEvents().Last();
            
            Assert.Equal(_userId, @event.UserId);
            Assert.Equal(_userName, @event.UserName);
        }
    }
}