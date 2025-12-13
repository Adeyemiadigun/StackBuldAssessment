using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Queries.Products
{
    public record ProductDto
    {
        public string Name { get;  set; } = null!;
        public string Description { get;  set; } = null!;
        public decimal Price { get;  set; }
        public int StockQuantity { get;  set; }
    }

    public static class ProductExtension
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
            };
        }
    }
}
