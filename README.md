# CSV Loader App (`CsvLoaderApp`)

A lightweight C# (.NET 10) console application that demonstrates how to read, validate, parse, and query data from a CSV file into strongly-typed objects without external dependencies.

---

## Project Structure

```text
CsvLoaderApp/
├── data/
│   └── products.csv        # Sample product inventory CSV file
├── CsvLoaderApp.csproj     # Project configuration & file-copy rules
├── Product.cs              # Domain model representing a single CSV row
├── Program.cs              # Main entry point: file reading, parsing & output
└── README.md               # Project documentation and starter guide
```

---

## Code Rationale & Design Decisions

### 1. Reliable File Path Resolution (`AppContext.BaseDirectory` + `.csproj` Copy Rule)
A common pitfall in console applications is using a relative path like `"data/products.csv"`, which relies on the user's current working directory (`Directory.GetCurrentDirectory()`) and fails if the executable is launched from another folder or IDE configuration.
- **`CsvLoaderApp.csproj`**: Configures `data\products.csv` with `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` so MSBuild automatically copies the CSV file into the build output directory (`bin/Debug/net10.0/data/products.csv`) whenever it changes.
- **`Program.cs`**: Uses `Path.Combine(AppContext.BaseDirectory, "data", "products.csv")` to resolve the file path relative to the compiled executable's directory, ensuring cross-platform compatibility (`/` vs `\`) and consistent execution regardless of where the command is run from.

### 2. Strongly-Typed Domain Model (`Product.cs`)
Instead of working with raw string arrays throughout the application:
- **`Product`** encapsulates the CSV columns (`Id`, `Name`, `Category`, `Price`, `StockQuantity`) with appropriate data types (`int`, `string`, `decimal`).
- **`decimal` for Currency**: `Price` uses `decimal` rather than `double` or `float` to avoid base-2 floating-point rounding errors in financial calculations.
- **Formatted `ToString()` Override**: Centralizes fixed-width column alignment (`{Name,-28}`, `{Price,8:C}`) so printing any `Product` instance automatically renders a clean table row.

### 3. Defensive Parsing & Validation (`Program.cs`)
Real-world CSV files often contain blank lines, missing fields, or typos:
- **Header Skipping**: Iteration starts at index `1` (`for (int i = 1; i < lines.Length; i++)`) to skip the header row (`Id,Name,Category,Price,StockQuantity`).
- **Empty Line Guard**: `string.IsNullOrWhiteSpace(line)` safely ignores trailing newlines or blank rows.
- **Column Count Check**: Ensures each row has at least 5 comma-separated columns before indexing into the array, preventing `IndexOutOfRangeException`.
- **`TryParse` over `Parse`**: Uses `int.TryParse` and `decimal.TryParse` instead of `int.Parse` so a single malformed row logs a helpful warning with its 1-based line number (`i + 1`) and skips the bad record rather than crashing the entire program.

### 4. Culture-Aware Formatting (`CultureInfo("en-GB")`)
Setting `CultureInfo.CurrentCulture = new CultureInfo("en-GB")` at startup ensures:
- Currency format strings (`:C`) consistently output pounds (`£`).
- `decimal.TryParse` with `NumberStyles.Currency` parses decimal values predictably across machines with different regional settings.

### 5. Querying with LINQ
Once loaded into `List<Product>`, the data can be queried and aggregated using standard LINQ methods—demonstrated by calculating the total inventory value with `products.Sum(p => p.Price * p.StockQuantity)`.

---

## Starter Guide & Commands

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or later)

Check your installed SDK version:
```bash
dotnet --version
```

### Quick Start Commands

1. **Clone the repository and navigate to the project folder:**
   ```bash
   git clone <repository-url>
   cd CsvLoaderApp
   ```

2. **Build and run the application:**
   ```bash
   dotnet run
   ```

3. **Build only (compiles code and copies `data/products.csv` to `bin/`):**
   ```bash
   dotnet build
   ```

4. **Clean build artifacts (`bin/` and `obj/`):**
   ```bash
   dotnet clean
   ```

### Expected Output

```text
Successfully loaded 5 products:

ID    Name                         | Category     |    Price | Stock
----------------------------------------------------------------------
[101] Wireless Mouse               | Accessories  |   £24.99 | Stock: 150
[102] Mechanical Keyboard          | Accessories  |   £89.50 | Stock: 85
[103] 27-inch 4K Monitor           | Displays     |  £349.00 | Stock: 40
[104] USB-C Docking Station        | Accessories  |  £129.99 | Stock: 60
[105] Noise-Cancelling Headphones  | Audio        |  £199.95 | Stock: 30
----------------------------------------------------------------------
Total Inventory Value: £39,112.40
```

---

## Ideas for Experimentation & Extension

- **Test Error Handling**: Add a malformed row (e.g., `106,Broken Item,Accessories,not-a-price,10`) to `data/products.csv` and run `dotnet run` to see how the parser reports and skips invalid lines.
- **Filter & Sort with LINQ**: Try filtering products by category (`products.Where(p => p.Category == "Accessories")`) or ordering by price (`products.OrderByDescending(p => p.Price)`).
- **Upgrade to `CsvHelper`**: Note that `string.Split(',')` is great for simple CSVs, but if your data contains quoted fields with embedded commas (e.g., `"Monitor, 27-inch"`), consider installing the popular NuGet package:
  ```bash
  dotnet add package CsvHelper
  ```

