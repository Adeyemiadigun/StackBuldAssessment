using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Model;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, DataResponse<AuthResponse>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwt;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(
            IRepository<User> userRepo,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwt,
            ILogger<LoginHandler> logger)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
            _logger = logger;
        }

        public async Task<DataResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.dto.Email);
            var user = await _userRepo.FirstOrDefaultAsync(u => u.Email == request.dto.Email, ct);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for email {Email}", request.dto.Email);
                throw new ApiException("Invalid credentials", 401, "InvalidCredentials");
            }
            var valid = _passwordHasher.VerifyPassword(
                request.dto.Password,
                user.PasswordHash,
                user.PasswordSalt);

            if (!valid)
            {
                _logger.LogWarning("Login failed: invalid password for email {Email}", request.dto.Email);
                throw new ApiException("Invalid credentials", 401, "InvalidCredentials");
            }

            var token = _jwt.GenerateToken(user);
            _logger.LogInformation("Login successful for user id {UserId}", user.Id);

            return new DataResponse<AuthResponse>
            {
                Data = new AuthResponse(
                token,
                DateTime.UtcNow.AddHours(1)),
                Message = "Login Successfully",
                Success = true

            };
        }
    }

}
