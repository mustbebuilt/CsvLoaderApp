using System.Globalization;
using CsvLoaderApp;

// Set culture to UK English (en-GB) so currency formatting (:C) uses £
CultureInfo.CurrentCulture = new CultureInfo("en-GB");

// 1. Define the path to the CSV file inside the /data folder
// AppContext.BaseDirectory ensures we look in the build output folder where data/products.csv was copied
string filePath = Path.Combine(AppContext.BaseDirectory, "data", "products.csv");

if (!File.Exists(filePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: Could not find CSV file at '{filePath}'");
    Console.ResetColor();
    return;
}

List<Product> products = new();

// 2. Read the file line by line
string[] lines = File.ReadAllLines(filePath);

// Skip the first line (index 0) because it contains the column headers:
// Id,Name,Category,Price,StockQuantity
for (int i = 1; i < lines.Length; i++)
{
    string line = lines[i];

    // Skip empty or whitespace-only lines
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    // 3. Split the line by comma
    string[] columns = line.Split(',');

    // Validate that the row has the expected 5 columns
    if (columns.Length < 5)
    {
        Console.WriteLine($"Skipping malformed row on line {i + 1}: {line}");
        continue;
    }

    // 4. Safely parse numeric values
    if (!int.TryParse(columns[0].Trim(), out int id) ||
        !decimal.TryParse(columns[3].Trim(), NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal price) ||
        !int.TryParse(columns[4].Trim(), out int stockQuantity))
    {
        Console.WriteLine($"Skipping row with invalid numbers on line {i + 1}: {line}");
        continue;
    }

    // 5. Create the Product object and add it to the list
    Product product = new()
    {
        Id = id,
        Name = columns[1].Trim(),
        Category = columns[2].Trim(),
        Price = price,
        StockQuantity = stockQuantity
    };

    products.Add(product);
}

// 6. Display the loaded data
Console.WriteLine($"Successfully loaded {products.Count} products:\n");
Console.WriteLine("ID    Name                         | Category     |    Price | Stock");
Console.WriteLine(new string('-', 70));

foreach (Product product in products)
{
    Console.WriteLine(product);
}

// 7. Example: Perform a simple LINQ calculation on the loaded data
decimal totalInventoryValue = products.Sum(p => p.Price * p.StockQuantity);
Console.WriteLine(new string('-', 70));
Console.WriteLine($"Total Inventory Value: {totalInventoryValue:C}");
