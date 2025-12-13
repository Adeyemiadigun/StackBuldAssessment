using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Models;
using MediatR;

namespace Application.Commands.Payment.InitailizePayment
{
    public record InitiatePaymentCommand(
    Guid OrderId,
    string Email
) : IRequest<DataResponse<PaymentInitResult>>;

}
