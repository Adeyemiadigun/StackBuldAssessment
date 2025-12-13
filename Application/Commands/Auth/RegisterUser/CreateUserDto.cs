using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Auth.RegisterUser
{
    public record CreateUserDto(string Email,
    string Password);
}
