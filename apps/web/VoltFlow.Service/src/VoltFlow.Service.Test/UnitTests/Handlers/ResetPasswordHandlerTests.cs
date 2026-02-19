using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Infrastructure.Handlers.Auth;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class ResetPasswordHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock; 
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;        
        private readonly IConfiguration _configuration;           
        private readonly ResetPasswordHandler _handler;

        public ResetPasswordHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["ClientSettings:BaseUrl"]).Returns("https://test-link.pl");
            _configuration = configMock.Object;

       
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            _handler = new ResetPasswordHandler(
                _userRepositoryMock.Object,
                _tokenRepositoryMock.Object,
                _tokenServiceMock.Object,
                _emailSenderMock.Object,
                _unitOfWorkMock.Object,
                _configuration);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenEmailIsSent()
        {
            // Arrange
            var email = "user@test.pl";
            var command = new ResetPasswordCommand(email);
            var user = new User { Id = 1, Email = email };
            var generatedToken = "test-token-123";

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateToken()).Returns(generatedToken);

            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                user.Email,
                EmailType.PasswordReset,
                It.IsAny<Dictionary<string, string>>(),
                user.Id,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();

            _emailSenderMock.Verify(x => x.SendTemplatedEmailAsync(
                user.Email,
                EmailType.PasswordReset,
                It.Is<Dictionary<string, string>>(d => d["ResetLink"].Contains(generatedToken)),
                user.Id,
                It.IsAny<CancellationToken>()), Times.Once);
        }

    [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var command = new ResetPasswordCommand ("nonexistent@test.pl");
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("User not found.");

            _tokenServiceMock.Verify(x => x.GenerateToken(), Times.Never);
            _emailSenderMock.Verify(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<EmailType>(), It.IsAny<object>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailSendingFails()
        {
            // Arrange
            var email = "user@test.pl";
            var command = new ResetPasswordCommand(email);
            var user = new User { Id = 1, Email = email };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateToken()).Returns("raw-token");
            _tokenServiceMock.Setup(x => x.HashToken(It.IsAny<string>())).Returns("hashed-token");

            // DODANE: Mocki dla repozytorium tokenów i UnitOfWork Save
            _tokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<VerificationToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Symulujemy błąd wysyłki
            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<EmailType>(), // Upewnij się, że to ten sam Enum co w Handlerze!
                It.IsAny<object>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Failed to send email.");

            // Weryfikacja transakcji
            _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
        }
    }
}
