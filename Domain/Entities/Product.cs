namespace Domain.Entities
{
    public class Product : BaseClass
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsDeleted
        public DateTime CreatedAt { get; private set; }

        public byte[]? RowVersion { get; private set; }

        private Product() { }

        public Product(string name, string description, decimal price, int stockQuantity)
        {
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            CreatedAt = DateTime.Now;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
            if (StockQuantity < quantity) throw new InvalidOperationException("Insufficient stock");
            StockQuantity -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
            StockQuantity += quantity;
        }
        public bool IsInStock(int quantity) 
            { return StockQuantity <= quantity; }
        public bool UpdateProduct(string name, string description, decimal price, int stockQuantity)
        {
            Name = name;
            Description = description;
            if (price < 0)
               throw new ArgumentException("Price must be > 0");
            Price = price;
            if (StockQuantity < 0)
                throw new ArgumentException("Quantity must be > 0");
            StockQuantity = stockQuantity;
            return true;
        }
    }

}
