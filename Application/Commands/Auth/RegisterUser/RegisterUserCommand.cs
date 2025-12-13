using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands.Auth.RegisterUser
{
    public record RegisterUserCommand(CreateUserDto CreateUser) : IRequest<Guid>;
}
