using System;
using System.Linq;
using Xunit;

namespace Brutus.User.Domain.Tests.User
{
    public class UserChangeNameTests
    {
        private readonly Domain.User _user;
        private readonly (Guid Id, string FirstName, string LastName, string Email) _userData;
        private readonly string _userFirstName = "Test First Name CHANGED";
        private readonly string _userLastName = "Test Last Name CHANGED";

        public UserChangeNameTests()
        {
            _userData = (Id: Guid.NewGuid(), FirstName: "Test First Name", LastName: "Test Last Name", Email: "test@email.com");
            _user = new Domain.User(_userData.Id, _userData.FirstName, _userData.LastName, _userData.Email);
        }

        [Fact]
        public void ShouldChangeAggregateDataOnSuccess()
        {
            _user.ChangeName(_userFirstName, _userLastName);
            
            Assert.Equal(_userFirstName, _user.FirstName);
            Assert.Equal(_userLastName, _user.LastName);
        }

        [Fact]
        public void ShouldCreateUserNameChangedEventOnSuccess()
        {
            _user.ChangeName(_userFirstName, _userLastName);
            var events = _user.DequeueEvents();
            Assert.Equal(typeof(Events.V1.UserNameChanged), events.Last().GetType());
        }
        
        [Fact]
        public void UserNameChangedEventShouldContainCorrectDataOnSuccess()
        {
            _user.ChangeName(_userFirstName, _userLastName);
            var @event = (Events.V1.UserNameChanged)_user.DequeueEvents().Last();
            
            Assert.Equal(_userData.Id, @event.UserId);
            Assert.Equal(_userFirstName, @event.FirstName);
            Assert.Equal(_userLastName, @event.LastName);
        }
        
        [Fact]
        public void ShouldTrimStartAndEndSpacesInNames()
        {
            var spacedFirstName = $"   {_userFirstName}   ";
            var spacedLastName = $"   {_userLastName}   ";
            
            _user.ChangeName(spacedFirstName, spacedLastName);
            
            Assert.Equal(_userFirstName, _user.FirstName);
            Assert.Equal(_userLastName, _user.LastName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ShouldThrowErrorIfFirstNameIsInvalid(string firstName)
        {
            Action act = () => _user.ChangeName(firstName, _userLastName);
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("FirstName could not be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldThrowErrorIfFirstNameIsLongerThen100Chars()
        {
            var firstName = "TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_TEST_LONG_NAME__";
            Action act = () => _user.ChangeName(firstName, _userLastName);
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("FirstName could not be longer the 100 characters", exception.Message);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ShouldThrowErrorIfLastNameIsInvalid(string lastName)
        {
            Action act = () => _user.ChangeName(_userLastName, lastName);
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("LastName could not be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldThrowErrorIfLastNameIsLongerThen100Chars()
        {
            var lastName = "TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_LONG_NAME__TEST_TEST_LONG_NAME__";
            Action act = () => _user.ChangeName(_userFirstName, lastName);
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("LastName could not be longer the 100 characters", exception.Message);
        }
    }
}