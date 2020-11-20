using System;
using System.Linq;
using Brutus.Core;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserActivateTests
    {
        private readonly Domain.User _user;
        private readonly (Guid Id, string Password, string Email) _userData;

        public UserActivateTests()
        {
            _userData = (Id: Guid.NewGuid(), Password: "Testing123!", Email: "initial_user@email.com");
            _user = new Domain.User(_userData.Id, _userData.Password, _userData.Email);
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
            var exception = Assert.Throws<DomainException>(() => _user.Activate());
            Assert.Equal("User could not be activated as it is already in Active status", exception.Message);
        }

        [Fact]
        public void ShouldThrowAnExceptionIfUserWasNotInPendingState()
        {
            _user.Activate();
            var exception = Assert.Throws<DomainException>(() => _user.Activate());
            Assert.Equal("User could not be activated as it is already in Active status", exception.Message);
        }
    }
}