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

namespace Application.Commands.Auth.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(
            IRepository<User> userRepo,
            IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var existing = await _userRepo.ExistsAsync(u => u.Email == request.CreateUser.Email, ct);

            if (existing)
                throw new ApiException("Email already exists", 409, "EmailExists");

            var (hash, salt) = _passwordHasher.HashPassword(request.CreateUser.Password);

            var user = new User(
                request.CreateUser.Email,
                hash,
                salt,
                UserRole.Customer);

            await _userRepo.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return user.Id;
        }
    }

}
