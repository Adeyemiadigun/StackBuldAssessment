namespace Application.Commands.Products.CreateProductCommand
{
    public record CreateProductDto(string Name, string Description, decimal Price, int StockQuantity);
}
