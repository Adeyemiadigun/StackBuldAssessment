using System.Transactions;
using Domain.Enums;

namespace Domain.Entities;

public class Order : BaseClass
{
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(x=>x.UnitPrice);
    private readonly List<PaymentTransaction> paymentTransactions = new();

    public IReadOnlyCollection<PaymentTransaction> Transactions => paymentTransactions;

    private Order() { } // EF

    public Order(IEnumerable<OrderItem> items,Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        _items.AddRange(items);
    }
    public void MarkAsPaid()
    {
        Status = OrderStatus.Paid;
    }
    public void MarkAsFailed() { Status = OrderStatus.Failed; }
}