using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Model;
using MediatR;

namespace Application.Commands.Auth.Login
{
    public record LoginCommand(LoginDto dto
 ) : IRequest<DataResponse<AuthResponse>>;

}
