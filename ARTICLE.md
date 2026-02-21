# Stop Writing Fat Repositories with RepoDb.Specifications: Master the Specification Pattern for RepoDB

![Banner Image](https://via.placeholder.com/1200x400?text=RepoDb.Specifications)

## The Problem: Fat Repositories Are Still Everywhere

If you've been building .NET applications for more than a few years, you've probably encountered—or even written—a repository class that looks like this:

```csharp
public class InvoiceRepository
{
    private readonly IDbConnection _connection;

    public InvoiceRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    // Query 1
    public IEnumerable<Invoice> GetActiveInvoices()
    {
        return _connection.Query<Invoice>(
            where: new QueryGroup(new[] { 
                new QueryField(nameof(Invoice.IsActive), true) 
            })
        );
    }

    // Query 2
    public IEnumerable<Invoice> GetRecentInvoices(int daysBack)
    {
        var fromDate = DateTime.Now.AddDays(-daysBack);
        return _connection.Query<Invoice>(
            where: new QueryGroup(new[] {
                new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate),
                new QueryField(nameof(Invoice.IsActive), true)
            })
        );
    }

    // Query 3
    public IEnumerable<Invoice> GetHighValueInvoices(decimal amount)
    {
        return _connection.Query<Invoice>(
            where: new QueryGroup(new[] {
                new QueryField(nameof(Invoice.Total), Operation.GreaterThan, amount),
                new QueryField(nameof(Invoice.IsActive), true)
            })
        );
    }

    // Query 4
    public IEnumerable<Invoice> GetInvoicesByCustomer(string customerName, int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        return _connection.Query<Invoice>(
            where: new QueryGroup(new[] {
                new QueryField(nameof(Invoice.CustomerName), customerName)
            }),
            orderBy: new[] { new OrderField(nameof(Invoice.CreatedDate), Order.Descending) },
            skip: skip,
            take: pageSize
        );
    }

    // ... 20 more methods like this ...
}
```

**The problems are evident:**

- 🔄 **Repetition**: Similar query logic duplicated across dozens of methods
- 🧪 **Hard to Test**: Testing individual query logic requires mocking the entire repository
- 😰 **Difficult to Evolve**: Adding a filter to three queries means changing three methods
- 🔗 **Tight Coupling**: Query logic is tightly bound to the repository implementation
- 📈 **Unmaintainable**: A 500+ line repository class becomes a maintenance nightmare

## The Solution: The Specification Pattern

The Specification Pattern encapsulates query logic into reusable, composable objects. Instead of creating a new repository method for each query variant, you define specifications that describe *what* to query, leaving the *how* to the repository.

### Here's the same logic with Specifications:

```csharp
// Define specifications
public class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
{
    public ActiveInvoicesSpec()
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Invoice.IsActive), true)
        }));
    }
}

public class RecentInvoicesSpec : RepoDbSpecification<Invoice>
{
    public RecentInvoicesSpec(int daysBack)
    {
        var fromDate = DateTime.Now.AddDays(-daysBack);
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate),
            new QueryField(nameof(Invoice.IsActive), true)
        }));
    }
}

public class HighValueInvoicesSpec : RepoDbSpecification<Invoice>
{
    public HighValueInvoicesSpec(decimal minimumAmount)
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Invoice.Total), Operation.GreaterThan, minimumAmount),
            new QueryField(nameof(Invoice.IsActive), true)
        }));
    }
}

// Lean repository
public class InvoiceRepository
{
    private readonly IDbConnection _connection;

    public InvoiceRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Invoice> GetBySpec(IRepoDbSpecification<Invoice> spec)
    {
        return _connection.Query(spec);
    }
}

// Usage
var activeSpec = new ActiveInvoicesSpec();
var invoices = repository.GetBySpec(activeSpec);
```

**Now you get:**

- ✅ **Reusability**: Define once, use everywhere
- ✅ **Testability**: Test specifications independently of repository
- ✅ **Composability**: Combine specifications with AND logic
- ✅ **Loose Coupling**: Query logic is separate from data access
- ✅ **Maintainability**: Easy to understand and modify

## Enter RepoDb.Specifications

While the Specification Pattern is powerful, implementing it correctly with RepoDB requires careful consideration of its API. That's where **jgalviz.RepoDb.Specifications** comes in.

### What is jgalviz.RepoDb.Specifications?

A lightweight, battle-tested NuGet package that provides:

- 📦 **Clean Abstractions**: `RepoDbSpecification<T>` base class for your specifications
- 🔗 **Composition**: Combine specifications with AND logic using the `.And()` method
- 🔍 **Filtering**: Support for WHERE clauses via `QueryGroup` and `QueryField`
- 📊 **Sorting**: ORDER BY via `OrderBy(field, direction)`
- 📄 **Projection**: SELECT specific columns via `Select(fields)`
- 🔢 **Paging**: SKIP/TAKE via `Page(skip, take)`
- 📈 **Helpers**: `Count<T>()` and `Exists<T>()` extension methods
- ✅ **100% Documented**: Full XML documentation and examples

### Installation

```bash
dotnet add package jgalviz.RepoDb.Specifications
```

Or via NuGet Package Manager:

```
Install-Package jgalviz.RepoDb.Specifications
```

## Real-World Example: E-Commerce System

Let's build a practical e-commerce system using jgalviz.RepoDb.Specifications.

### Define Your Specifications

```csharp
// Basic filtering
public class ActiveProductsSpec : RepoDbSpecification<Product>
{
    public ActiveProductsSpec()
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Product.IsActive), true)
        }));
    }
}

// Parameterized filtering
public class ProductsByCategorySpec : RepoDbSpecification<Product>
{
    public ProductsByCategorySpec(string category)
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Product.Category), category),
            new QueryField(nameof(Product.IsActive), true)
        }));
        
        OrderBy(nameof(Product.Name), SortDirection.Asc);
    }
}

// Complex business logic
public class LowStockProductsSpec : RepoDbSpecification<Product>
{
    public LowStockProductsSpec(int threshold = 10)
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Product.StockQuantity), Operation.LessThanOrEqual, threshold),
            new QueryField(nameof(Product.IsActive), true)
        }));

        Select(
            nameof(Product.ProductId),
            nameof(Product.Name),
            nameof(Product.StockQuantity)
        );

        OrderBy(nameof(Product.StockQuantity), SortDirection.Asc);
    }
}

// Paged results
public class ProductSearchSpec : RepoDbSpecification<Product>
{
    public ProductSearchSpec(string category, decimal maxPrice, int pageNumber = 1, int pageSize = 20)
    {
        var fields = new List<QueryField>
        {
            new QueryField(nameof(Product.Category), category),
            new QueryField(nameof(Product.IsActive), true)
        };

        if (maxPrice > 0)
        {
            fields.Add(new QueryField(nameof(Product.Price), Operation.LessThanOrEqual, maxPrice));
        }

        Where(new QueryGroup(fields.ToArray()));
        
        OrderBy(nameof(Product.Name), SortDirection.Asc);
        
        int skip = (pageNumber - 1) * pageSize;
        Page(skip, pageSize);
    }
}
```

### Use in Your Repository

```csharp
public class ProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    // Simple query
    public IEnumerable<Product> GetActive()
    {
        return _connection.Query(new ActiveProductsSpec());
    }

    // Category search with paging
    public IEnumerable<Product> Search(string category, decimal maxPrice, int page, int pageSize)
    {
        var spec = new ProductSearchSpec(category, maxPrice, page, pageSize);
        return _connection.Query(spec);
    }

    // Inventory management
    public long CountLowStock(int threshold)
    {
        return _connection.Count(new LowStockProductsSpec(threshold));
    }

    // Alert system
    public bool HasCriticalLowStock()
    {
        return _connection.Exists(new LowStockProductsSpec(threshold: 5));
    }
}
```

### Usage in Your Business Logic

```csharp
public class InventoryService
{
    private readonly ProductRepository _repository;

    public InventoryService(ProductRepository repository)
    {
        _repository = repository;
    }

    public void CheckAndAlertLowStock()
    {
        if (_repository.HasCriticalLowStock())
        {
            // Trigger reorder process
            Console.WriteLine("🚨 ALERT: Critical low stock detected!");
            // Send notifications, create purchase orders, etc.
        }
    }

    public void DisplayDashboard()
    {
        var lowStockCount = _repository.CountLowStock(threshold: 20);
        var activeProducts = _repository.GetActive();
        
        Console.WriteLine($"Dashboard:");
        Console.WriteLine($"  Active Products: {activeProducts.Count()}");
        Console.WriteLine($"  Low Stock Items: {lowStockCount}");
    }
}
```

## Key Advantages Over Fat Repositories

### 1. **Specification Composition**

```csharp
// Combine multiple specifications with AND
var spec = new ActiveProductsSpec()
    .And(new ProductsByCategorySpec("Electronics"))
    .And(new PriceLessThanSpec(500));

var electronics = _connection.Query(spec);
```

### 2. **Reusability Across Layers**

The same specification can be used in:
- Repository queries
- Service business logic
- API filtering
- Reports
- Background jobs
- Unit tests

```csharp
// Test
[Fact]
public void LowStockAlert_ShouldTrigger_WhenProductsBelowThreshold()
{
    var spec = new LowStockProductsSpec(threshold: 10);
    var result = _connection.Query(spec);
    
    Assert.NotEmpty(result);
}

// Service
public void ProcessLowStockAlert()
{
    var spec = new LowStockProductsSpec(threshold: 10);
    var lowStockItems = _connection.Query(spec);
    // Process reorder
}
```

### 3. **Easy Testing**

```csharp
[Fact]
public void ActiveProductsSpec_FiltersCorrectly()
{
    var spec = new ActiveProductsSpec();
    var query = _connection.Query(spec);
    
    Assert.All(query, p => Assert.True(p.IsActive));
}

[Fact]
public void LowStockSpec_SelectsOnlyRequiredFields()
{
    var spec = new LowStockProductsSpec();
    var query = _connection.Query(spec);
    
    // Verify query includes SelectFields
    Assert.NotNull(spec.SelectFields);
    Assert.Equal(3, spec.SelectFields.Count);
}
```

### 4. **Performance Optimization**

Project only the columns you need:

```csharp
// Without projection - loads ALL columns
var allData = _connection.Query<Product>(/* ... */);

// With projection - loads only what you need
public class ProductListSpec : RepoDbSpecification<Product>
{
    public ProductListSpec()
    {
        Select(
            nameof(Product.ProductId),
            nameof(Product.Name),
            nameof(Product.Price)
        );
    }
}
var listData = _connection.Query(new ProductListSpec()); // Faster!
```

## Architecture Benefits

### Domain-Driven Design (DDD)

Specifications fit naturally into DDD:

```csharp
// In Domain/Specifications folder
public class OutstandingOrdersSpec : RepoDbSpecification<Order>
{
    public OutstandingOrdersSpec(CustomerId customerId)
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Order.CustomerId), customerId.Value),
            new QueryField(nameof(Order.Status), OrderStatus.Pending)
        }));
    }
}

// In Application layer
public class OrderService
{
    private readonly OrderRepository _repository;

    public void NotifyCustomerOfOutstandingOrders(CustomerId customerId)
    {
        var spec = new OutstandingOrdersSpec(customerId);
        var orders = _repository.GetBySpec(spec);
        
        foreach (var order in orders)
        {
            SendNotification(order);
        }
    }
}
```

### Clean Architecture

Specifications keep your architecture clean:

```
Domain/
  └── Specifications/
      ├── ActiveInvoicesSpec.cs
      ├── OverdueInvoicesSpec.cs
      └── HighValueInvoicesSpec.cs

Application/
  └── Services/
      ├── InvoiceService.cs
      └── BillingService.cs

Infrastructure/
  └── Persistence/
      ├── InvoiceRepository.cs
      └── DbConnectionFactory.cs
```

## Real-World Production Patterns

### 1. Dashboard Metrics

```csharp
public class DashboardService
{
    private readonly IDbConnection _connection;

    public DashboardMetrics GetMetrics()
    {
        return new DashboardMetrics
        {
            PendingOrders = _connection.Count(new PendingOrdersSpec()),
            OverdueInvoices = _connection.Count(new OverdueInvoicesSpec()),
            LowStockItems = _connection.Count(new LowStockProductsSpec()),
            NewCustomersThisMonth = _connection.Count(new NewCustomersThisMonthSpec())
        };
    }
}
```

### 2. Alerting System

```csharp
public class AlertService
{
    private readonly IDbConnection _connection;

    public void CheckAndAlert()
    {
        if (_connection.Exists(new OverdueInvoicesSpec()))
        {
            var count = _connection.Count(new OverdueInvoicesSpec());
            SendAlert($"⚠️  {count} invoices are overdue");
        }

        if (_connection.Exists(new LowStockProductsSpec(threshold: 10)))
        {
            SendAlert("🚨 Critical low stock detected");
        }
    }
}
```

### 3. Reporting

```csharp
public class ReportService
{
    private readonly IDbConnection _connection;

    public void GenerateMonthlySalesReport(int year, int month)
    {
        var spec = new MonthlySalesSpec(year, month);
        var sales = _connection.Query(spec);
        
        ExportToExcel(sales, $"Sales_{year}_{month}.xlsx");
    }
}
```

## Common Pitfalls and Solutions

### ❌ Pitfall 1: Over-Generalization

**Don't do this:**

```csharp
public class GenericFilterSpec : RepoDbSpecification<Invoice>
{
    // ...tries to handle ALL scenarios
    public GenericFilterSpec(string status, bool? isActive, decimal? minAmount, 
        int? daysBack, string customerName, int page, int pageSize)
    {
        // 50 lines of conditional logic
    }
}
```

**Do this instead:**

```csharp
public class ActiveInvoicesSpec : RepoDbSpecification<Invoice> { }
public class RecentInvoicesSpec : RepoDbSpecification<Invoice> { }
public class HighValueInvoicesSpec : RepoDbSpecification<Invoice> { }

// Compose them
var spec = new ActiveInvoicesSpec()
    .And(new RecentInvoicesSpec(30))
    .And(new HighValueInvoicesSpec(1000));
```

### ❌ Pitfall 2: Specifications That Do Too Much

**Don't do this:**

```csharp
public class InvoiceSpec : RepoDbSpecification<Invoice>
{
    public void ApplyBusinessRules() { }
    public decimal CalculateTax() { }
    public void SendNotification() { }
}
```

**Do this instead:**

```csharp
public class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
{
    // Only filtering, sorting, paging, projection
}

// Business logic goes elsewhere
public class InvoiceService
{
    public void ProcessInvoices()
    {
        var spec = new ActiveInvoicesSpec();
        var invoices = _repository.GetBySpec(spec);
        
        // Business logic here
        foreach (var invoice in invoices)
        {
            ApplyBusinessRules(invoice);
        }
    }
}
```

## Getting Started

### Step 1: Install Package

```bash
dotnet add package jgalviz.RepoDb.Specifications
```

### Step 2: Create Your First Specification

```csharp
using RepoDb;

public class ActiveProductsSpec : RepoDbSpecification<Product>
{
    public ActiveProductsSpec()
    {
        Where(new QueryGroup(new[] {
            new QueryField(nameof(Product.IsActive), true)
        }));
    }
}
```

### Step 3: Use in Repository

```csharp
var spec = new ActiveProductsSpec();
var products = connection.Query(spec);
```

### Step 4: Compose Multiple Specifications

```csharp
var spec = new ActiveProductsSpec()
    .And(new ProductsByCategorySpec("Electronics"))
    .And(new AffordablePriceSpec(500));

var results = connection.Query(spec);
```

## Conclusion

The Specification Pattern is not new, but it's incredibly powerful when implemented correctly. By using **jgalviz.RepoDb.Specifications**, you get:

- ✅ **Cleaner code** - No more fat repositories
- ✅ **Better testability** - Test specifications independently
- ✅ **Reusable logic** - Use the same specification everywhere
- ✅ **Easier maintenance** - Query logic is centralized
- ✅ **DDD alignment** - Fits naturally in domain-driven designs
- ✅ **Performance** - Column projection built-in

## Resources

- **NuGet Package**: https://www.nuget.org/packages/jgalviz.RepoDb.Specifications
- **GitHub Repository**: https://github.com/jgalviz1974/RepoDb.Specifications
- **Examples**: Full working examples included in the repository
- **Contributing**: Open to contributions and feedback

---

**Ready to stop writing fat repositories?** Try jgalviz.RepoDb.Specifications today and master the Specification Pattern in your .NET applications.

### Next Steps

1. Install the NuGet package
2. Review the [examples](https://github.com/jgalviz1974/RepoDb.Specifications/tree/main/examples)
3. Start building your first specifications
4. Share your feedback on [GitHub](https://github.com/jgalviz1974/RepoDb.Specifications)

Happy coding! 🚀

---

**Have questions or feedback?** Open an [issue](https://github.com/jgalviz1974/RepoDb.Specifications/issues) or start a [discussion](https://github.com/jgalviz1974/RepoDb.Specifications/discussions) on GitHub.
