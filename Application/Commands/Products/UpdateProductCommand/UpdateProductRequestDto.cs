using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Commands.Products.UpdateProductCommand
{
    public record UpdateProductRequestDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string? Description { get;  set; } 
        public decimal Price { get;  set; }
        public int StockQuantity { get;  set; }
    }

    public static class UpdateProductToProduct
    {
        public static Product ToProduct(this UpdateProductRequestDto product)
        {
            return new Product(product.ProductName, product.Description, product.Price, product.StockQuantity);
            
        }
    }
}
