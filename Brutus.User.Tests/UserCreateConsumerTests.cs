using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.CommandHandlers;
using Brutus.User.Domain;
using MassTransit;
using MassTransit.Testing;
using Moq;
using Xunit;

namespace Brutus.User.Tests
{
    public class UserCreateConsumerTests:ConsumerTestBase
    {
        private readonly ConsumerTestHarness<UserCreateCommandHandler> _consumer;
        public UserCreateConsumerTests()
        {
            var userMock = new Mock<IRepository<Domain.User>>();
            userMock.Setup(repo => repo.AddAsync(It.IsAny<Domain.User>())).Returns<Aggregate>(user => Task.FromResult(user.DequeueEvents()));
            
            _consumer = Harness.Consumer(() => new UserCreateCommandHandler(userMock.Object));
        }
        
        [Fact]
        public async Task ShouldPublishTheUserCreatedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserCreate
                {
                    UserId = InVar.Id,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                    Email = "test@email.com"
                });

                Assert.True(await Harness.Consumed.Any<Domain.Commands.V1.UserCreate>());
                Assert.True(await _consumer.Consumed.Any<Domain.Commands.V1.UserCreate>());

                Assert.True(await Harness.Published.Any<Domain.Events.V1.UserCreated>());
                Assert.False(await Harness.Published.Any<Fault<Domain.Commands.V1.UserCreate>>());
            });
        }

        [Fact]
        public async Task ShouldCreateFaultOnException()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserCreate()
                {
                    UserId = InVar.Id,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                    Email = "@email.com"
                });
                
                Assert.True(await Harness.Published.Any<Fault<Domain.Commands.V1.UserCreate>>());
            });
        }
    }
}
