using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Infrastructure.Handlers.Auth;

namespace VoltFlow.Service.Test.UnitTests.Handlers
{
    public class LoginHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJWTProvider> _jwtProviderMock;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            // 1. Setup UserManager Mock
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            // 2. Setup SignInManager Mock
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null);

            // 3. Reszta zależności
            _jwtProviderMock = new Mock<IJWTProvider>();

            _handler = new LoginHandler(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _jwtProviderMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new LoginCommand { Email = "nonexistent@test.com", Password = "AnyPassword" };


            // Safest way for MockQueryable:
            var usersList = new List<User>(); // Empty list
            var usersMock = usersList.BuildMock(); // Call BuildMock() on LISTS (IEnumerable)

            _userManagerMock.Setup(x => x.Users).Returns(usersMock);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeFalse();
            result._Message.Should().Be("Invalid credentials.");
        }

        [Fact]
        public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "valid@test.com",
                Password = "CorrectPassword123!"
            };

            // 1. Przygotowujemy dane z rolą
            var user = new User
            {
                Email = command.Email,
                Role = new Role { Name = "User" }
            };

            // 2. We use BuildMock() instead of manually setting Providers.
            // This resolves the IAsyncQueryProvider error for .Include() and .FirstOrDefaultAsync()
            var usersMock = new List<User> { user }.BuildMock();

            _userManagerMock.Setup(x => x.Users).Returns(usersMock);

            // 3. We mock the password check result.
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(
            It.IsAny<User>(),
            command.Password,
            false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // 4. We mock token generation
            _jwtProviderMock.Setup(x => x.Generate(It.IsAny<User>()))
                .Returns("fake-jwt-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result._IsSuccess.Should().BeTrue();
            result._Data.Token.Should().Be("fake-jwt-token");
        }
    }
}
