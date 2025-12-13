using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public record AuthResponse(string AccessToken,
    DateTime ExpiresAt);
}
