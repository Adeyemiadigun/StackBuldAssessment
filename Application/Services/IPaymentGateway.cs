using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Services
{
    public interface IPaymentGateway
    {
        Task<PaymentInitResult> InitiateAsync(
            decimal amount,
            string email,
            string reference,
            CancellationToken ct);

        Task<PaymentVerifyResult> VerifyAsync(
            string reference,
            CancellationToken ct);
    }

}
