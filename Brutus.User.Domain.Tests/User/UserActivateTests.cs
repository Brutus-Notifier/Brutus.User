using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserActivateTests
    {
        private readonly Domain.User _user;
        private readonly (Guid Id, string FirstName, string LastName, string Email) _userData;

        public UserActivateTests()
        {
            _userData = (Id: Guid.NewGuid(), FirstName: "User First Name", LastName: "User Last Name", Email: "initial_user@email.com");
            _user = new Domain.User(_userData.Id, _userData.FirstName, _userData.LastName, _userData.Email);
        }

        [Fact]
        public void ShouldCreateUserActivatedEventOnSucceed()
        {
            _user.Activate();
            var @event = _user.DequeueEvents().Last();
            Assert.NotNull(@event);
            Assert.Equal(typeof(Events.V1.UserActivated), @event.GetType());
        }
        
        
        [Fact]
        public void ShouldHaveCorrectInformationInCreatedEvent()
        {
            _user.Activate();
            var @event = (Events.V1.UserActivated) _user.DequeueEvents().Last();
            Assert.Equal(_user.Email, @event.Email);
            Assert.Equal(_user.FirstName, @event.FirstName);
            Assert.Equal(_user.LastName, @event.LastName);
            Assert.Equal(_user.Id, @event.UserId);
        }

        [Fact]
        public void ShouldChangeStatusOfAggregateToActive()
        {
            _user.Activate();
            Assert.Equal(Domain.User.UserStatus.Active, _user.Status);
        }

        [Fact]
        public void ShouldThrowAnExceptionIfUserIsAlreadyActivated()
        {
            _user.Activate();
            var exception = Assert.Throws<AggregateException>(() => _user.Activate());
            Assert.Equal("User could not be activated as it is already in Active status", exception.Message);
        }

        [Fact]
        public void ShouldThrowAnExceptionIfUserWasNotInPendingState()
        {
            _user.Activate();
            var exception = Assert.Throws<AggregateException>(() => _user.Activate());
            Assert.Equal("User could not be activated as it is already in Active status", exception.Message);
        }
    }
}