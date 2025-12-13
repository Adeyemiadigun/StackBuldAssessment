using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using MediatR;

namespace Application.Commands.Payment.VerifyPayment
{
    public record VerifyPaymentCommand(string Reference) : IRequest<DataResponse<VerificationData>>;

}
