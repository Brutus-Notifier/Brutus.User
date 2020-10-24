using System;
using System.Linq;
using System.Threading.Tasks;
using Brutus.User.Sagas;
using MassTransit;
using Xunit;

namespace Brutus.User.Tests
{
    public class UserRegistrationTestBaseSagaTests: TestBaseSaga<UserRegistrationState, UserRegistrationSaga>
    {
        private readonly Guid _userId;
        private readonly string _userEmail;

        public UserRegistrationTestBaseSagaTests()
        {
            _userId = NewId.NextGuid();
            _userEmail = "test@email.com";
        }
        
        [Fact]
        public async Task ShouldChangeStateToSubmittedOnUserCreatedEvent()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", _userEmail));
                var instance = await FindMachineInstance(_userId, StateMachine.Submitted);
                
                Assert.NotNull(instance);
                Assert.True(Harness.Consumed.Select<Domain.Events.V1.UserCreated>().Any());
                Assert.True(HarnessSaga.Consumed.Select<Domain.Events.V1.UserCreated>().Any());
            });
        } 
        
        [Fact]
        public async Task ShouldChangeStateToConfirmationSentOnEmailConfirmationSentEvent()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", _userEmail), TimeSpan.FromMilliseconds(100));
                await Publish(new Events.V1.UserEmailConfirmationSent(_userId, _userEmail));

                var instance = await FindMachineInstance(_userId, StateMachine.ConfirmationSent);
                
                Assert.NotNull(instance);
                Assert.Equal(_userEmail, instance.Email);
                
                Assert.True(Harness.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());
                Assert.True(HarnessSaga.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());
                
            });
        }
        
        [Fact]
        public async Task ShouldChangeStateToFinishedEmailConfirmedEvent()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", _userEmail), TimeSpan.FromMilliseconds(100));
                await Publish(new Events.V1.UserEmailConfirmationSent(_userId, _userEmail));
                await Publish(new Domain.Events.V1.UserEmailConfirmed(_userId, _userEmail));

                var instance = await FindMachineInstance(_userId, StateMachine.Final);
                
                Assert.NotNull(instance);
                Assert.True(Harness.Consumed.Select<Domain.Events.V1.UserEmailConfirmed>().Any());
                Assert.True(HarnessSaga.Consumed.Select<Domain.Events.V1.UserEmailConfirmed>().Any());
            });
        }
        
        [Fact]
        public async Task ShouldPublishSendConfirmationCommand()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", _userEmail));
                Assert.True(await Harness.Published.Any<Commands.V1.UserSendEmailConfirmation>());
            });
        }
    }
}