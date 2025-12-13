using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Payment.InitailizePayment
{
    public class InitiatePaymentHandler
    : IRequestHandler<InitiatePaymentCommand, DataResponse<PaymentInitResult>>
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<PaymentTransaction> _paymentTransactionRepo; 
        private readonly IPaymentGateway _paymentGateway;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        public InitiatePaymentHandler(IRepository<Order> orderRepo,IPaymentGateway paymentGateway, IRepository<PaymentTransaction> paymentTransactionRepo, ICurrentUser currentUser, ILogger logger, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _paymentGateway = paymentGateway;
            _paymentTransactionRepo = paymentTransactionRepo;
            _currentUser = currentUser;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<DataResponse<PaymentInitResult>> Handle(
            InitiatePaymentCommand request,
            CancellationToken ct)
        {
            _logger.LogInformation(
                "Initializing payment for Order {OrderId}",
                request.OrderId);
            var user = await _currentUser.GetUserAsync();
            
            var order = await _orderRepo.GetByIdAsync(request.OrderId,ct)
                ?? throw new ApiException("Order not found", 404, "OrderNotFound");

            var reference = $"Order-APP--{Guid.NewGuid().ToString("N")[..8]}";

            var result = await _paymentGateway.InitiateAsync(
                order.TotalAmount,
                request.Email,
                reference,
                ct);

            var payment = new PaymentTransaction(
                order.Id,
                user,
                order.TotalAmount,
                "Paystack",
                reference);

            await _paymentTransactionRepo.AddAsync(payment, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation(
               "Order payment record created with ref {Reference} for Order {App}",
               reference, request.OrderId);


            return new DataResponse<PaymentInitResult>
            {
                Success = true,
                Message = "Payment Initialized",
                Data = new PaymentInitResult(
                result.Success,
                result.AuthorizationUrl,
                reference)
            };

        }
    }

}
