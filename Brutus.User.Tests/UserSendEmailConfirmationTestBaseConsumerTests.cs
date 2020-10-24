using System;
using System.Linq;
using System.Threading.Tasks;
using Brutus.User.CommandHandlers;
using Brutus.User.Services;
using MassTransit;
using MassTransit.Events;
using MassTransit.Testing;
using Moq;
using Xunit;

namespace Brutus.User.Tests
{
    public class UserSendEmailConfirmationTestBaseConsumerTests: TestBaseConsumer
    {
        private readonly ConsumerTestHarness<UserSendConfirmationEmailCommandHandler> _consumer;

        public UserSendEmailConfirmationTestBaseConsumerTests()
        {
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            
            _consumer = Harness.Consumer(() => new UserSendConfirmationEmailCommandHandler(emailServiceMock.Object));
        }

        [Fact]
        public async Task ShouldPublishFaultOnEmailEmpty()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.UserSendEmailConfirmation
                {
                    UserId = InVar.Id,
                    Email = ""
                });

                await Harness.Published.Any();
                var publishedEvent = Harness.Published.Select<Fault<Commands.V1.UserSendEmailConfirmation>>().SingleOrDefault();
                var faultEventException = ((FaultEvent<Commands.V1.UserSendEmailConfirmation>) publishedEvent.MessageObject).Exceptions[0];

                Assert.NotNull(publishedEvent);
                Assert.Equal("Value cannot be null. (Parameter 'Email')" ,faultEventException.Message);
            });
        }
        
        [Fact]
        public async Task ShouldPublishFaultOnUserIdEmpty()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.UserSendEmailConfirmation
                {
                    UserId = Guid.Empty,
                    Email = "test@email.com"
                });

                await Harness.Published.Any();
                var publishedEvent = Harness.Published.Select<Fault<Commands.V1.UserSendEmailConfirmation>>().SingleOrDefault();
                var faultEventException = ((FaultEvent<Commands.V1.UserSendEmailConfirmation>) publishedEvent.MessageObject).Exceptions[0];

                Assert.NotNull(publishedEvent);
                Assert.Equal("Value cannot be null. (Parameter 'UserId')" ,faultEventException.Message);
            });
        }

        [Fact]
        public async Task ShouldPublishTheUserNameChangedEvent()
        {
            await RunTest(async () =>
            {
                await Harness.InputQueueSendEndpoint.Send(new Commands.V1.UserSendEmailConfirmation
                {
                    UserId = InVar.Id,
                    Email = "test@email.com"
                });
                
                Assert.True(await Harness.Consumed.Any<Commands.V1.UserSendEmailConfirmation>());
                Assert.True(await _consumer.Consumed.Any<Commands.V1.UserSendEmailConfirmation>());
                
                Assert.True(await Harness.Published.Any<Events.V1.UserEmailConfirmationSent>());
                Assert.False(await Harness.Published.Any<Fault<Commands.V1.UserSendEmailConfirmation>>());
            });
        }
    }
}