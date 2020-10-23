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
    public class UserConfirmEmailConsumerTests: ConsumerTestBase
    {
        private readonly ConsumerTestHarness<UserConfirmEmailCommandHandler> _consumer;

        public UserConfirmEmailConsumerTests()
        {
            var userMock = new Mock<IRepository<Domain.User>>();
            userMock.Setup(repo => repo.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.User(InVar.Id, "testName", "testLastName", "test@email.com"));
            userMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.User>())).Returns<Aggregate>(user => Task.FromResult(user.DequeueEvents()));

            _consumer = Harness.Consumer(() => new UserConfirmEmailCommandHandler(userMock.Object));
        }

        [Fact]
        public async Task ShouldPublishUserEmailConfirmedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserConfirmEmail()
                {
                    UserId = InVar.Id,
                    Email = "test@email.com"
                });
                
                Assert.True(await Harness.Consumed.Any<Domain.Commands.V1.UserConfirmEmail>());
                Assert.True(await _consumer.Consumed.Any<Domain.Commands.V1.UserConfirmEmail>());
                
                Assert.True(await Harness.Published.Any<Domain.Events.V1.UserEmailConfirmed>());
                Assert.False(await Harness.Published.Any<Fault<Domain.Commands.V1.UserConfirmEmail>>());
            });
        }
        
        [Fact]
        public async Task ShouldCreateFaultOnException()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Domain.Commands.V1.UserConfirmEmail()
                {
                    UserId = InVar.Id,
                    Email = "    "
                });
                
                Assert.True(await Harness.Published.Any<Fault<Domain.Commands.V1.UserConfirmEmail>>());
            });
        }
    }
}