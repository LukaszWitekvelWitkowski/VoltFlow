using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Infrastructure.Handlers.Auth
{
    public class LoginHandler : IRequestHandler<LoginCommand,ServiceResponse<TokenResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJWTProvider _jwtProvider;

        public LoginHandler(UserManager<User> userManager, SignInManager<User> signInManager, IJWTProvider jwtProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<ServiceResponse<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                            .Include(u => u.Role) 
                            .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
            {
                return ServiceResponse<TokenResponse>.Failure("Invalid credentials.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!signInResult.Succeeded)
            {
                return ServiceResponse<TokenResponse>.Failure("Invalid credentials.");
            }

            var token = _jwtProvider.Generate(user);

            return ServiceResponse<TokenResponse>.Success(new TokenResponse(token));

        }
    }
}
