namespace CatalogService.Models;

public class Product
{
    public required int Id { get; set; }    
    public required string Name { get; set; }
    
    public double Price { get; set; }
    
    public int Stock { get; set; }
}