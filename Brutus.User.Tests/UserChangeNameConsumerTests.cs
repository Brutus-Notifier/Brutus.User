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
    public class UserChangeNameConsumerTests:ConsumerTestBase
    {
        private readonly ConsumerTestHarness<ChangeUserNameCommandHandler> _consumer;
        
        public UserChangeNameConsumerTests()
        {
            var userMock = new Mock<IRepository<Domain.User>>();
            userMock.Setup(repo => repo.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.User(InVar.Id, "testName", "testLastName", "test@email.com"));
            userMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.User>())).Returns<Aggregate>(user => Task.FromResult(user.DequeueEvents()));
            
            _consumer = Harness.Consumer(() => new ChangeUserNameCommandHandler(userMock.Object));
        }
        
        [Fact]
        public async Task ShouldPublishTheUserNameChangedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.ChangeUserName
                {
                    UserId = InVar.Id,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                });

                Assert.True(await Harness.Consumed.Any<Commands.V1.ChangeUserName>());
                Assert.True(await _consumer.Consumed.Any<Commands.V1.ChangeUserName>());

                Assert.True(await Harness.Published.Any<Events.V1.UserNameChanged>());
                Assert.False(await Harness.Published.Any<Fault<Commands.V1.ChangeUserName>>());
            });
        }

        [Fact]
        public async Task ShouldCreateFaultOnException()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.ChangeUserName
                {
                    UserId = InVar.Id,
                    FirstName = "",
                    LastName = "TestLastName",
                });
                
                Assert.True(await Harness.Published.Any<Fault<Commands.V1.ChangeUserName>>());
            });
        }
    }
}
