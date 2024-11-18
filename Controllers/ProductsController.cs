using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CatalogService.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly CatalogContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(CatalogContext context, IDistributedCache cache, ILogger<ProductsController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var cacheKey = $"Product_{id}";
        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            _logger.LogInformation($"Fetching product with ID={id} from cache.");
            return Ok(System.Text.Json.JsonSerializer.Deserialize<Product>(cachedProduct));
        }

        _logger.LogInformation($"Fetching product with ID={id} from database.");
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Cache the product for 10 minutes
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(product), options);

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}