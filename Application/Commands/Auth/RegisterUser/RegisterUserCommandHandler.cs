using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Auth.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(
            IRepository<User> userRepo,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            ILogger<RegisterUserHandler> logger)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", request.CreateUser.Email);

            var existing = await _userRepo.ExistsAsync(u => u.Email == request.CreateUser.Email, ct);

            if (existing)
            {
                _logger.LogWarning("Registration failed: email already exists: {Email}", request.CreateUser.Email);
                throw new ApiException("Email already exists", 409, "EmailExists");
            }

            var (hash, salt) = _passwordHasher.HashPassword(request.CreateUser.Password);

            var user = new User(
                request.CreateUser.Email,
                hash,
                salt,
                UserRole.Customer);

            await _userRepo.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("User registration successful. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

            return user.Id;
        }
    }
}
