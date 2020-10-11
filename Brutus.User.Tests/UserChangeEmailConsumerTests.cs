using System;
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
    public class UserChangeEmailConsumerTests:ConsumerTestBase
    {
        private readonly ConsumerTestHarness<ChangeUserEmailCommandHandler> _consumer;
        
        public UserChangeEmailConsumerTests()
        {
            var userMock = new Mock<IRepository<Domain.User>>();
            userMock.Setup(repo => repo.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.User(InVar.Id, "testName", "testLastName", "test@email.com"));
            userMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.User>())).Returns<IAggregate>(user => Task.FromResult(user.DequeueEvents()));
            
            _consumer = Harness.Consumer(() => new ChangeUserEmailCommandHandler(userMock.Object));
        }
        
        [Fact]
        public async Task ShouldPublishTheUserNameChangedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.ChangeUserEmail
                {
                    UserId = InVar.Id,
                    Email = "test2@email.com"
                });

                Assert.True(await Harness.Consumed.Any<Commands.V1.ChangeUserEmail>());
                Assert.True(await _consumer.Consumed.Any<Commands.V1.ChangeUserEmail>());

                Assert.True(await Harness.Published.Any<Events.V1.UserEmailChanged>());
                Assert.False(await Harness.Published.Any<Fault<Commands.V1.ChangeUserEmail>>());
            });
        }

        [Fact]
        public async Task ShouldCreateFaultOnException()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.ChangeUserEmail
                {
                    UserId = InVar.Id,
                    Email = "@aweqwe@qweqwe.com"
                });
                
                Assert.True(await Harness.Published.Any<Fault<Commands.V1.ChangeUserEmail>>());
            });
        }
    }
}
