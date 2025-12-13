namespace Domain.Entities;

public class OrderItem : BaseClass
{
    public Guid ProductId { get; private set; }

    public Product? Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Guid OrderId { get; private set; }

    private OrderItem() { } // EF

    public OrderItem(Guid productId,Guid orderId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        OrderId = orderId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

