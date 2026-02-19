using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Infrastructure.Data;
using VoltFlow.Service.Infrastructure.Handlers.Auth;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class RegisterUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly VoltFlowDbContext _dbContext;

        public RegisterUserHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var options = new DbContextOptionsBuilder<VoltFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new VoltFlowDbContext(options);
        }
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "exists@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Login = "testuser"
            };

            var command = new RegisterUserCommand(request);

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.RegisterUserRequest.Email))
                .ReturnsAsync(new User()); // Symulujemy, że user istnieje

            var handler = new RegisterUserHndler(_userManagerMock.Object, _dbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Użytkownik już istnieje.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_AndCommitTransaction_WhenDataIsValid()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "exists@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Login = "testuser"
            };

            var command = new RegisterUserCommand(request);

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.RegisterUserRequest.Email))
                .ReturnsAsync((User)null); // User nie istnieje

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), command.RegisterUserRequest.Password))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new RegisterUserHndler(_userManagerMock.Object, _dbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();
            _userManagerMock.Verify(x => x.CreateAsync(It.Is<User>(u => u.Email == command.RegisterUserRequest.Email), command.RegisterUserRequest.Password), Times.Once);
        }



        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenIdentityReturnsErrors()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "exists@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Login = "testuser"
            };

            var command = new RegisterUserCommand(request);

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.RegisterUserRequest.Email)).ReturnsAsync((User)null);

            // ROZWIĄZANIE DLA IdentityErrorDescriber:
            var describer = new IdentityErrorDescriber();
            var identityError = describer.DefaultError();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var handler = new RegisterUserHndler(_userManagerMock.Object, _dbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse(); // Teraz zadziała
            result._Message.Should().NotBeNullOrEmpty(); // Teraz zadziała
        }
    }
}
