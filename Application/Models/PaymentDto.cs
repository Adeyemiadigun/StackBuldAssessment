using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public record PaymentInitResult(
    bool Success,
    string AuthorizationUrl,
    string Reference);

    public record PaymentVerifyResult(
        bool Success,
        string Status,
        decimal Amount,
        string Reference);

}
