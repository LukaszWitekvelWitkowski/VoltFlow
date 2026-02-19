using FluentAssertions;
using Moq;
using VoltFlow.Service.Application.Commands.Email;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Infrastructure.Handlers.Email;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class SendEmailCommandtHandlerTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly SendOverduePaymentHandler _handler;

        public SendEmailCommandtHandlerTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _handler = new SendOverduePaymentHandler(_emailSenderMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldPassExactCommandData_ToEmailSender()
        {
            // Arrange
            var command = new SendEmailCommand(
                ClientId: 1,
                CustomerEmail: "customer@example.com",
                CustomerName: "Jan Kowalski",
                DueDate: DateTime.UtcNow.AddDays(-5)
            );

            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<EmailType>(),
                It.IsAny<object>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();

       
            _emailSenderMock.Verify(x => x.SendTemplatedEmailAsync(
                command.CustomerEmail,
                EmailType.OverduePayment,
                command, // Handler przekazuje cały obiekt jako model
                command.ClientId,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailSenderFails()
        {
            // Arrange
            var command = new SendEmailCommand(1, "test@test.pl", "Test", DateTime.Now);

            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<EmailType>(),
                It.IsAny<object>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Failed to send message.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task Handle_ShouldReturnFailure_WhenAmountIsZeroOrNegative(decimal invalidAmount)
        {
            // Arrange
            var command = new SendEmailCommand(
                ClientId: 1,
                CustomerEmail: "test@test.pl",
                CustomerName: "Test",
                DueDate: DateTime.Now.AddDays(-1)
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Contain("Failed to send message."); 
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailSenderThrowsException()
        {
            // Arrange
            var command = new SendEmailCommand(1, "test@test.pl", "Test", DateTime.Now);

            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<EmailType>(), It.IsAny<object>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("SMTP Timeout"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);


            result._Message.Should().Contain("SMTP Timeout");
        }

        [Fact]
        public async Task Handle_ShouldEnsureClientIdIsPassedCorrectly_ForAuditPurposes()
        {
            // Arrange
            int expectedClientId = 999;
            var command = new SendEmailCommand(expectedClientId, "audit@test.pl", "Auditor", DateTime.Now);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<EmailType>(),
                It.IsAny<object>(),
                expectedClientId,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
