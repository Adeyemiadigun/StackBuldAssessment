using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Payment.VerifyPayment
{
    public record VerificationData(
         Guid OrderId,
         decimal AmountPaid,
         string Status,
         string TransactionReference,
         DateTime? PaidAt);

}
