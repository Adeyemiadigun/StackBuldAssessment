using System.Transactions;

namespace Domain.Entities;

public class Order : BaseClass
{
    public DateTime CreatedAt { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    private readonly List<PaymentTransaction> paymentTransactions = new();

    public IReadOnlyCollection<PaymentTransaction> Transactions => paymentTransactions;

    private Order() { } // EF

    public Order(IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        _items.AddRange(items);
    }
}