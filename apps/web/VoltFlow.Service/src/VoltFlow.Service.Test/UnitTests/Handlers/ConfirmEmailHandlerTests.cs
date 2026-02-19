
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth.Request;
using VoltFlow.Service.Infrastructure.Handlers.Auth;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class ConfirmEmailHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly ConfirmEmailHandler _handler;

        public ConfirmEmailHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _emailSenderMock = new Mock<IEmailSender>();
            _configurationMock = new Mock<IConfiguration>();

            // UserManager wymaga specjalnego traktowania w Moq
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _handler = new ConfirmEmailHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _emailSenderMock.Object,
                _tokenRepositoryMock.Object,
                _userManagerMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new ConfirmEmailCommand(new ConfirmEmailRequest { Email = "notfound@example.com" });
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Unable to find user.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyVerified()
        {
            // Arrange
            var user = new User { Email = "test@example.com", IsEmailVerified = true };
            var command = new ConfirmEmailCommand(new ConfirmEmailRequest { Email = user.Email });
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Email is already verified.");
        }


        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailSenderFails()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", IsEmailVerified = false };
            var command = new ConfirmEmailCommand(new ConfirmEmailRequest { Email = user.Email });

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateToken()).Returns("raw-token");
            _tokenServiceMock.Setup(x => x.HashToken(It.IsAny<string>())).Returns("hashed-token");

            // Symulujemy błąd wysyłki
            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<EmailType>(), It.IsAny<object>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Failed to send verification message.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenEverythingIsCorrect()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", IsEmailVerified = false, Name = "John" };
            var command = new ConfirmEmailCommand(new ConfirmEmailRequest { Email = user.Email });

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateToken()).Returns("raw-token");
            _tokenServiceMock.Setup(x => x.HashToken(It.IsAny<string>())).Returns("hashed-token");
            _emailSenderMock.Setup(x => x.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<EmailType>(), It.IsAny<object>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();
            _tokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<VerificationToken>(), It.IsAny<CancellationToken>()), Times.Once);
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.IsSendEmailVeryfied == true)), Times.Once);
        }
    }
}
