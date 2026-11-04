namespace CsvLoaderApp;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public override string ToString()
    {
        return $"[{Id}] {Name,-28} | {Category,-12} | {Price,8:C} | Stock: {StockQuantity}";
    }
}

