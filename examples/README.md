# Examples - jgalviz.RepoDb.Specifications

Complete working examples demonstrating how to use jgalviz.RepoDb.Specifications in real-world scenarios.

## 📚 Files Overview

### 1. **ExampleEntities.cs**
Base entity classes used throughout all examples:
- `Invoice` - Billing/invoicing entity
- `Order` - E-commerce order entity
- `Product` - Inventory/product entity
- `Customer` - Customer management entity

### 2. **BasicFilteringExamples.cs**
Simple filtering examples for beginners:
- Filter active invoices
- Filter by status
- Filter by amount threshold
- Filter by date range
- Filter products by category

**Key Concepts:**
- Creating specifications with WHERE clauses
- Using `QueryField` with different operations
- Basic specification usage with `connection.Query()`

### 3. **CompositionExamples.cs**
Combining specifications with AND logic:
- Basic composition pattern
- Two-way composition
- E-commerce order filtering
- Practical business scenarios

**Key Concepts:**
- Using `.And()` method to combine specifications
- Chaining multiple specifications
- Composition for complex queries

### 4. **SortingAndPaginationExamples.cs**
Sorting and pagination patterns:
- Single column sorting
- Multiple column sorting
- Page-by-page results
- Top N results pattern
- Dashboard queries

**Key Concepts:**
- Using `OrderBy()` for sorting
- `Page()` method for pagination
- Skip/Take calculations
- Limiting results for performance

### 5. **ProjectionExamples.cs**
Column selection (SELECT) patterns:
- List display projection
- Minimal summaries
- Export data selection
- Report generation
- API response formatting
- Dropdown/autocomplete queries

**Key Concepts:**
- Using `Select()` to specify columns
- Performance benefits of projection
- Different projections for different purposes
- Memory and bandwidth optimization

### 6. **CountAndExistsExamples.cs**
Using Count and Exists helpers:
- Counting matching records
- Existence checks
- Business logic patterns
- Dashboard metrics
- Inventory alerts
- Overdue payment notifications

**Key Concepts:**
- Using `connection.Count(spec)` for counts
- Using `connection.Exists(spec)` for existence checks
- Conditional business logic
- Alert and notification patterns

### 7. **RealWorldExamples.cs**
Production-ready business scenarios:

#### E-Commerce Scenarios
- Product search and filtering
- Customer order history
- Order fulfillment operations
- Sales reporting

#### Billing/Invoicing Scenarios
- Accounts receivable (overdue payment collection)
- Revenue recognition reporting

#### Customer Management
- High-value customer identification
- New customer tracking

#### Inventory Management
- Low stock alert system
- Reorder triggering

## 🚀 Quick Start

### Running Examples

Each file contains `UsageExample()` or `UsageExamples()` static methods that demonstrate how to use the patterns:

```csharp
using System.Data;
using Examples;

// Assuming you have a database connection
IDbConnection connection = GetConnection();

// Run basic filtering examples
BasicFilteringExamples.UsageExamples(connection);

// Run composition examples
CompositionExamples.CombineSpecificationsExample(connection);

// Run sorting and pagination
SortingAndPaginationExamples.PaginationExample(connection);

// Run projection examples
ProjectionExamples.UsageExamples(connection);

// Run count and exists examples
CountAndExistsExamples.CountExamples(connection);
CountAndExistsExamples.ExistsExamples(connection);

// Run real-world scenarios
RealWorldExamples.UsageScenarios.ECommerceExample(connection);
RealWorldExamples.UsageScenarios.BillingExample(connection);
RealWorldExamples.UsageScenarios.InventoryExample(connection);
```

## 📋 Pattern Reference

### Basic Filtering
```csharp
public class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
{
    public ActiveInvoicesSpec()
    {
        Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true)
        }));
    }
}
```

### With Sorting
```csharp
OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
```

### With Pagination
```csharp
int pageNumber = 2;
int pageSize = 20;
int skip = (pageNumber - 1) * pageSize;
Page(skip, pageSize);
```

### With Projection
```csharp
Select(
    nameof(Invoice.Id),
    nameof(Invoice.CustomerName),
    nameof(Invoice.Total)
);
```

### Composing Specifications
```csharp
var spec = new ActiveInvoicesSpec()
    .And(new RecentInvoicesSpec())
    .And(new HighValueInvoicesSpec(5000));
```

### Count and Exists
```csharp
long count = connection.Count(spec);
bool exists = connection.Exists(spec);
```

## 🎯 Use Case Matrix

| Use Case | File | Key Method |
|----------|------|-----------|
| Simple filtering | BasicFilteringExamples | `Where()` |
| Multiple criteria | CompositionExamples | `.And()` |
| Ordered results | SortingAndPaginationExamples | `OrderBy()` |
| Limit results | SortingAndPaginationExamples | `Page()` |
| Specific columns | ProjectionExamples | `Select()` |
| Count records | CountAndExistsExamples | `Count()` |
| Check existence | CountAndExistsExamples | `Exists()` |
| Real scenarios | RealWorldExamples | All patterns combined |

## 📊 Common Queries

### Get Top 10 Records
```csharp
public class Top10Spec : RepoDbSpecification<Invoice>
{
    public Top10Spec()
    {
        Where(new QueryGroup(new[] { 
            new QueryField(nameof(Invoice.IsActive), true) 
        }));
        OrderBy(nameof(Invoice.Total), SortDirection.Desc);
        Page(skip: 0, take: 10);
    }
}
```

### Get Page 2 with 20 Records per Page
```csharp
int pageNumber = 2;
int pageSize = 20;
int skip = (pageNumber - 1) * pageSize; // = 20
Page(skip: skip, take: pageSize);
```

### Filter by Date Range
```csharp
var startDate = new DateTime(2024, 1, 1);
var endDate = new DateTime(2024, 1, 31);

Where(new QueryGroup(new[]
{
    new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, startDate),
    new QueryField(nameof(Invoice.CreatedDate), Operation.LessThanOrEqual, endDate)
}));
```

### Select Specific Columns
```csharp
Select(
    nameof(Invoice.Id),
    nameof(Invoice.CustomerName),
    nameof(Invoice.Total),
    nameof(Invoice.CreatedDate)
);
```

### Combine AND Multiple Criteria
```csharp
var spec = new ActiveInvoicesSpec()
    .And(new RecentInvoicesSpec(30))
    .And(new HighValueInvoicesSpec(1000));
```

## 🔍 Learning Path

### Beginner
1. Start with `ExampleEntities.cs` to understand data model
2. Read `BasicFilteringExamples.cs` for simple WHERE clauses
3. Try `CountAndExistsExamples.cs` for basic operations

### Intermediate
4. Learn `CompositionExamples.cs` to combine specifications
5. Master `SortingAndPaginationExamples.cs` for result ordering
6. Use `ProjectionExamples.cs` for performance optimization

### Advanced
7. Study `RealWorldExamples.cs` for production patterns
8. Implement your own specifications following the patterns
9. Create domain-specific specifications for your business logic

## 💡 Best Practices

1. **One Spec Per Query Pattern** - Create specific specs for common queries
2. **Reuse Specifications** - Compose existing specs rather than creating new ones
3. **Name Clearly** - Use descriptive names like `ActiveInvoicesSpec` not `FilterSpec`
4. **Project Wisely** - Select only needed columns for performance
5. **Compose Thoughtfully** - Use AND to build complex queries
6. **Document** - Add XML comments to specifications
7. **Test** - Unit test specifications in isolation

## 📞 Support

For questions or issues:
- Check the main [README.md](../README.md)
- Visit [GitHub Issues](https://github.com/jgalviz1974/RepoDb.Specifications/issues)
- Start a [GitHub Discussion](https://github.com/jgalviz1974/RepoDb.Specifications/discussions)

## 📄 License

All examples are provided under the same [MIT License](../LICENSE) as the main project.

---

Happy coding! 🚀
