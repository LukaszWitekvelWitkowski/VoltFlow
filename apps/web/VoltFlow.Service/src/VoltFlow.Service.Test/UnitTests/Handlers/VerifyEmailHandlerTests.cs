using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Enums;
using VoltFlow.Service.Core.Models.Auth.Request;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Infrastructure.Handlers.Auth;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class VerifyEmailHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClientService> _clientServiceMock; // Dodane
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly VerifyEmailHandler _handler;

        public VerifyEmailHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clientServiceMock = new Mock<IClientService>(); // Inicjalizacja

            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _handler = new VerifyEmailHandler(
                _userRepositoryMock.Object,
                _tokenRepositoryMock.Object,
                _tokenServiceMock.Object,
                _userManagerMock.Object,
                _unitOfWorkMock.Object,
                _clientServiceMock.Object); // Wstrzyknięcie
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTokenAndUserAreValid()
        {
            // Arrange
            var request = new VerifyEmailRequest("test@example.com", "raw-token");
            var command = new VerifyEmailCommand(request);
            var user = new User { Id = 1, Email = request.Email, IsEmailVerified = false };
            var tokenRecord = VerificationToken.Create("hashed-token", 1, TokenType.EmailConfirmation);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.HashToken(request.Token)).Returns("hashed-token");

            _tokenRepositoryMock.Setup(x => x.GetActiveTokenAsync(user.Id, "hashed-token", TokenType.EmailConfirmation, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenRecord);

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();
            user.IsEmailVerified.Should().BeTrue();
            tokenRecord.IsUsed.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTokenIsInvalidOrExpired()
        {
            // Arrange
            var request = new VerifyEmailRequest("test@example.com", "wrong-token");
            var command = new VerifyEmailCommand(request);
            var user = new User { Id = 1, Email = request.Email };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.HashToken(It.IsAny<string>())).Returns("hashed-wrong-token");

            // Zwracamy null, co symuluje brak aktywnego tokena w DB
            _tokenRepositoryMock.Setup(x => x.GetActiveTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TokenType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VerificationToken)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Invalid or expired token.");
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once); // finally zawsze commituje w Twoim kodzie
        }

        [Fact]
        public async Task Handle_ShouldRollback_WhenExceptionOccurs()
        {
            // Arrange
            var request = new VerifyEmailRequest("test@example.com", "token");
            var command = new VerifyEmailCommand(request);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB connection error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._StatusCode.Should().Be(500);
            result._Message.Should().Contain("DB connection error");
            _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_AndCreateClient_WhenTokenAndUserAreValid()
        {
            // Arrange
            var request = new VerifyEmailRequest("test@example.com", "raw-token");
            var command = new VerifyEmailCommand(request);
            var user = new User { Id = 1, Email = request.Email, IsEmailVerified = false };
            var tokenRecord = VerificationToken.Create("hashed-token", 1, TokenType.EmailConfirmation);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.HashToken(request.Token)).Returns("hashed-token");
            _tokenRepositoryMock.Setup(x => x.GetActiveTokenAsync(user.Id, "hashed-token", TokenType.EmailConfirmation, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenRecord);

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Mockujemy sukces serwisu klienta
            _clientServiceMock.Setup(x => x.CreateClientFromUserAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(100); // Przykładowe ID klienta

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();
            user.IsEmailVerified.Should().BeTrue();
            tokenRecord.IsUsed.Should().BeTrue();

            // Kluczowe asercje dla Seniora:
            _clientServiceMock.Verify(x => x.CreateClientFromUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotCreateClient_WhenUserNotFound()
        {
            // Arrange
            var request = new VerifyEmailRequest("notfound@example.com", "any-token");
            var command = new VerifyEmailCommand(request);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            _clientServiceMock.Verify(x => x.CreateClientFromUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
