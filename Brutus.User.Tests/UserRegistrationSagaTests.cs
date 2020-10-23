using System;
using System.Linq;
using System.Threading.Tasks;
using Brutus.User.Sagas;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Brutus.User.Tests
{
    public class UserRegistrationSagaTests: ConsumerTestBase
    {
        private readonly UserRegistrationSaga _stateMachine;
        private readonly StateMachineSagaTestHarness<UserRegistrationState, UserRegistrationSaga> _harnessSaga;
        private Guid _userId;

        public UserRegistrationSagaTests()
        {
            _stateMachine = new UserRegistrationSaga();
            _harnessSaga = Harness.StateMachineSaga<UserRegistrationState, UserRegistrationSaga>(_stateMachine);
            _userId = NewId.NextGuid();
        }
        
        // [Fact]
        public async Task ShouldChangeStateToSubmittedOnUserCreatedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.Bus.Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", "test@email.com"));
                
                Assert.True(Harness.Consumed.Select<Domain.Events.V1.UserCreated>().Any());
                Assert.True(_harnessSaga.Consumed.Select<Domain.Events.V1.UserCreated>().Any());

                var instance = _harnessSaga.Created.ContainsInState(_userId, _stateMachine, _stateMachine.Submitted);
                Assert.NotNull(instance);
            });
        } 
        
        // [Fact]
        public async Task ShouldChangeStateToConfirmationSentOnEmailConfirmationSentEvent()
        {
            await RunTest(async () =>
            {
                await Harness.Bus.Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", "test@email.com"));
                await Harness.Bus.Publish(new Events.V1.UserEmailConfirmationSent(_userId, "test@email.com"));
                
                Assert.True(Harness.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());
                Assert.True(_harnessSaga.Consumed.Select<Events.V1.UserEmailConfirmationSent>().Any());

                var sagaInstance = _harnessSaga.Sagas.Select(x => x.CorrelationId == _userId).ToList();
                
                var instance = _harnessSaga.Created.ContainsInState(_userId, _stateMachine, _stateMachine.ConfirmationSent);

                Assert.NotNull(instance);
                Assert.Equal("test@email.com", instance.Email);
            });
        }
        
        // [Fact]
        // public async Task ShouldChangeStateToFinishedEmailConfirmedEvent()
        // {
        //     await RunTest(async () =>
        //     {
        //         await Harness.Bus.Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", "test@email.com"));
        //         await Harness.Bus.Publish(new Events.V1.UserEmailConfirmationSent(_userId, "test@email.com"));
        //         await Harness.Bus.Publish(new Domain.Events.V1.UserEmailConfirmed(_userId, "test@email.com"));
        //         
        //         Assert.True(Harness.Consumed.Select<Domain.Events.V1.UserEmailConfirmed>().Any());
        //         Assert.True(_harnessSaga.Consumed.Select<Domain.Events.V1.UserEmailConfirmed>().Any());
        //
        //         var instance = _harnessSaga.Created.ContainsInState(_userId, _stateMachine, _stateMachine.Final);
        //         Assert.NotNull(instance);
        //     });
        // }
        
        // [Fact]
        public async Task ShouldPublishSendConfirmationCommand()
        {
            await RunTest(async () =>
            {
                await Harness.Bus.Publish(new Domain.Events.V1.UserCreated(_userId, "TestFirst", "TestLast", "test@email.com"));
                Assert.True(await Harness.Published.Any<Commands.V1.UserSendEmailConfirmation>());
            });
        }
    }
}