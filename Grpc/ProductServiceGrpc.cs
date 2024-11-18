using CatalogService.Data;
using CatalogService.Models;
using Grpc.Core;

namespace CatalogService.Grpc;

public class ProductServiceGrpc : ProductService.ProductServiceBase
{ 
    private readonly CatalogContext _context;

    public ProductServiceGrpc(CatalogContext context)
    {
        _context = context;
    }

    public override async Task<Product> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var productFromDb = await _context.Products.FindAsync(request.Id);
        if (productFromDb == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID={request.Id} not found"));
        }

        return new Product
        {
            Id = productFromDb.Id,
            Name = productFromDb.Name,
            Price = (double)productFromDb.Price,
            Stock = productFromDb.Stock
        };
    }
}