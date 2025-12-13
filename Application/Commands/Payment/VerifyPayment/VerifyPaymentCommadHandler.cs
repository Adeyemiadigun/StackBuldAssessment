using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Payment.VerifyPayment
{
    public class VerifyPaymentHandler : IRequestHandler<VerifyPaymentCommand,DataResponse<VerificationData>>
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IRepository<PaymentTransaction> _paymentRepository;
        private readonly ILogger<VerifyPaymentHandler> _logger;
        private readonly IRepository<Order> _orderRepo;
        private readonly IUnitOfWork _unitOfWork;
        public VerifyPaymentHandler(IPaymentGateway paymentGateway,IRepository<PaymentTransaction> paymentRepo,ILogger<VerifyPaymentHandler> logger,IUnitOfWork unitOfWork,IRepository<Order> orderRepo)
        { 
            _paymentGateway = paymentGateway;
            _paymentRepository = paymentRepo;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
        }
        public async Task<DataResponse<VerificationData>> Handle(
            VerifyPaymentCommand request,
            CancellationToken ct)
        {
            _logger.LogInformation("Verifying payment with reference {Reference}", request.Reference);
            

            var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.Reference == request.Reference);
            if (payment == null)
            {
                _logger.LogWarning("Payment record not found for reference {Reference}", request.Reference);

                throw new ApiException("Payment not found", 404, "PaymentNotFound");
            }
            var order = await _orderRepo.GetByIdAsync(payment.OrderId,ct)!?? throw new ApiException("Order not found", 404, "OrderNotFound");


            
            var result = await _paymentGateway.VerifyAsync(request.Reference, ct);

            if (!result.Success)
            {
                payment.MarkFailed();
                order.MarkAsFailed();
                await _unitOfWork.SaveChangesAsync(ct);
                return new DataResponse<VerificationData> 
                { 
                    Success = false,
                    Message = "Payment Failed",
                    Data = new VerificationData(payment.Id, payment.Amount,result.Status,request.Reference,payment.CreatedAt)
                };
            }

            payment.MarkSuccessful();
            order.MarkAsPaid();

           await _unitOfWork.SaveChangesAsync(ct);
            return new DataResponse<VerificationData>
            {
                Success = true,
                Message = "Payment Successfull",
                Data = new VerificationData(payment.Id, payment.Amount, result.Status, request.Reference, payment.CreatedAt)
            };

        }
    }

}
