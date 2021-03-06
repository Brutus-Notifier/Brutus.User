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
        private readonly (Guid Id, string Password, string Email) _userData;
        private readonly Guid _invitationId;

        public UserRegistrationTestBaseSagaTests()
        {
            _userData = (NewId.NextGuid(), "Testing123!", "test@email.com");
            _invitationId = Guid.NewGuid();
        }
        
        [Fact]
        public async Task ShouldChangeStateToSubmittedOnUserCreatedEvent()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userData.Id, _userData.Password, _userData.Email));
                var instance = await FindMachineInstance(_userData.Id, StateMachine.Submitted);
                
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
                await Publish(new Domain.Events.V1.UserCreated(_userData.Id, _userData.Password, _userData.Email));
                await Publish(new Events.V1.UserEmailConfirmationSent(_userData.Id, _userData.Email));

                var instance = await FindMachineInstance(_userData.Id, StateMachine.ConfirmationSent);
                
                Assert.NotNull(instance);
                Assert.Equal(_userData.Email, instance.Email);
                
                Assert.True(Harness.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());
                Assert.True(HarnessSaga.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());
                
            });
        }
        
        [Fact]
        public async Task ShouldChangeStateToFinishedWhenUserActivatedEvent()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userData.Id, _userData.Password, _userData.Email));
                await Publish(new Events.V1.UserEmailConfirmationSent(_userData.Id, _userData.Email));
                await Publish(new Domain.Events.V1.UserActivated(_userData.Id));
                
                Assert.True(Harness.Consumed.Select<Domain.Events.V1.UserActivated>().Any());
                Assert.True(HarnessSaga.Consumed.Select<Domain.Events.V1.UserActivated>().Any());

                var instance = await FindMachineInstance(_userData.Id, StateMachine.Final);
                Assert.NotNull(instance);
            });
        }
        
        [Fact]
        public async Task ShouldPublishSendConfirmationCommand()
        {
            await RunTest(async () =>
            {
                await Publish(new Domain.Events.V1.UserCreated(_userData.Id, _userData.Password, _userData.Email));
                Assert.True(await Harness.Published.Any<Commands.V1.UserSendEmailConfirmation>());
            });
        }
    }
}