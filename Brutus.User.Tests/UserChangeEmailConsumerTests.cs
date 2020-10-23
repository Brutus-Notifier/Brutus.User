using System;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.CommandHandlers;
using MassTransit;
using MassTransit.Testing;
using Moq;
using Xunit;

namespace Brutus.User.Tests
{
    public class UserChangeEmailConsumerTests:ConsumerTestBase
    {
        private readonly ConsumerTestHarness<UserChangeEmailCommandHandler> _consumer;
        
        public UserChangeEmailConsumerTests()
        {
            var userMock = new Mock<IRepository<Domain.User>>();
            userMock.Setup(repo => repo.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.User(InVar.Id, "testName", "testLastName", "test@email.com"));
            userMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.User>())).Returns<Aggregate>(user => Task.FromResult(user.DequeueEvents()));
            
            _consumer = Harness.Consumer(() => new UserChangeEmailCommandHandler(userMock.Object));
        }
        
        [Fact]
        public async Task ShouldPublishTheUserNameChangedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserChangeEmail
                {
                    UserId = InVar.Id,
                    Email = "test2@email.com"
                });

                Assert.True(await Harness.Consumed.Any<Domain.Commands.V1.UserChangeEmail>());
                Assert.True(await _consumer.Consumed.Any<Domain.Commands.V1.UserChangeEmail>());

                Assert.True(await Harness.Published.Any<Domain.Events.V1.UserEmailChanged>());
                Assert.False(await Harness.Published.Any<Fault<Domain.Commands.V1.UserChangeEmail>>());
            });
        }

        [Fact]
        public async Task ShouldCreateFaultOnException()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserChangeEmail
                {
                    UserId = InVar.Id,
                    Email = "@aweqwe@qweqwe.com"
                });
                
                Assert.True(await Harness.Published.Any<Fault<Domain.Commands.V1.UserChangeEmail>>());
            });
        }
    }
}
