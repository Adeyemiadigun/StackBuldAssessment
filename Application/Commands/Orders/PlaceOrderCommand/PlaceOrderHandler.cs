using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Orders.PlaceOrderCommand;
    public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PlaceOrderHandler> _logger;

        public PlaceOrderHandler(
            IRepository<Product> productRepo,
            IRepository<Order> orderRepo,
            IUnitOfWork uow,ILogger<PlaceOrderHandler> logger)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _uow = uow;
            _logger = logger;
        }

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        const int MAX_RETRIES = 3;

        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                _logger.LogInformation("Place Order Transaction Begins - Attempt {Attempt}", attempt);

                var items = new List<OrderItem>();

                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId)
                        ?? throw new ApiException("Product not found", 404, "ProductNotFound");

                    product.DecreaseStock(item.Quantity);

                    _logger.LogInformation("Product {ProductId} stock decreased", product.Id);

                    items.Add(new OrderItem(
                        product.Id,
                        Guid.Empty,
                        item.Quantity,
                        product.Price));
                }

                var order = new Order(items);

                await _orderRepo.AddAsync(order, cancellationToken);

                await _uow.CommitAsync();

                _logger.LogInformation("Order Placed Successfully");
                return order.Id;
            }
            catch (ConcurrencyException ex)
            {
                await _uow.RollbackAsync();
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict detected on attempt {Attempt}. Retrying...",
                    attempt
                );

                if (attempt == MAX_RETRIES)
                {
                    _logger.LogError("Max retry attempts reached. Failing permanently.");
                    throw new ApiException(
                        "Another user updated stock while you were placing this order. Please try again.",
                        409,
                        "ConcurrencyConflict"
                    );
                }

                // small delay between retries  
                await Task.Delay(150, cancellationToken);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Order placement failed due to error");
                throw;
            }
        }
        throw new ApiException("Unknown Error", 500, "UKnownError");

    }


}
