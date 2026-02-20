# RepoDb.Specifications

A lightweight and expressive implementation of the **Specification Pattern for RepoDB**, designed to encapsulate query logic in a reusable, composable, and testable way.

This library provides first-class support for **filtering**, **sorting**, **paging**, and **projection**, using RepoDB-native primitives—without relying on `IQueryable` or LINQ-to-ORM abstractions.

---

## Why RepoDb.Specifications?

As applications grow, query logic tends to become:

- Duplicated across repositories  
- Hard to test  
- Difficult to evolve  
- Tightly coupled to infrastructure concerns  

**RepoDb.Specifications** allows you to model queries as reusable objects that describe *what to query*, while repositories focus only on *executing* those queries.

---

## Key Features

- ✅ Specification Pattern tailored for RepoDB  
- ✅ Native use of `QueryGroup` and `QueryField`  
- ✅ Filtering (WHERE)  
- ✅ Sorting (ORDER BY)  
- ✅ Paging (SKIP / TAKE)  
- ✅ Column projection and DTO mapping  
- ✅ No `IQueryable`, no hidden LINQ translation  
- ✅ Clean Architecture & DDD friendly  
- ✅ Provider-agnostic (SQL Server, MySQL, PostgreSQL, etc.)

---

## Installation

```bash
dotnet add package RepoDb.Specifications
```

---

## Core Concepts

A **Specification**:

- Describes a query  
- Is reusable and composable  
- Does not execute database operations  

Specifications are evaluated by repositories or extension methods.

---

## Example Entity

```csharp
public class Invoice
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public decimal Total { get; set; }
    public DateTime IssueDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
```

---

## Example 1: Simple Filtering Specification

```csharp
public sealed class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
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

---

## Example 2: Filtering + Sorting

```csharp
public sealed class RecentInvoicesSpec : RepoDbSpecification<Invoice>
{
    public RecentInvoicesSpec(DateTime fromDate)
    {
        Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true),
            new QueryField(
                nameof(Invoice.IssueDate),
                Operation.GreaterThanOrEqual,
                fromDate)
        }));

        OrderBy(nameof(Invoice.IssueDate), SortDirection.Desc);
    }
}
```

---

## Example 3: Filtering + Sorting + Paging

```csharp
public sealed class PagedInvoicesSpec : RepoDbSpecification<Invoice>
{
    public PagedInvoicesSpec(int page, int pageSize)
    {
        Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true)
        }));

        OrderBy(nameof(Invoice.IssueDate), SortDirection.Desc);

        var skip = (page - 1) * pageSize;
        Page(skip, pageSize);
    }
}
```

---

## Example 4: Column Projection (SELECT specific fields)

```csharp

public sealed class InvoiceListSpec : RepoDbSpecification<Invoice>
{
    public InvoiceListSpec()
    {
        Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true)
        }));

        Select(
            nameof(Invoice.Id),
            nameof(Invoice.IssueDate),
            nameof(Invoice.Total),
            nameof(Invoice.CustomerName)
        );
    }
}
```

---

## Example 5: Using a Specification in a Repository

```csharp
public sealed class InvoiceRepository
{
    private readonly IDbConnection _connection;

    public InvoiceRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Invoice> List(IRepoDbSpecification<Invoice> spec)
    {
        return _connection.Query(spec);
    }
}
```

### Usage

```csharp
var spec = new PagedInvoicesSpec(page: 1, pageSize: 20);
var invoices = invoiceRepository.List(spec);
```

---

## Example 6: DTO Projection

```csharp
public sealed class InvoiceListItemDto
{
    public long Id { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal Total { get; set; }
}
```

```csharp
var spec = new InvoiceListSpec();

IEnumerable<InvoiceListItemDto> result =
    connection.QueryProjected<InvoiceListItemDto>(spec);
```

---

## Example 7: Composing Specifications with AND

```csharp
// Create individual specifications
var activeSpec = new ActiveInvoicesSpec();
var recentSpec = new RecentInvoicesSpec(fromDate: DateTime.Now.AddMonths(-1));

// Combine them using AND logic
var combinedSpec = activeSpec.And(recentSpec);

// Use the combined specification
var invoices = connection.Query(combinedSpec);
```

**How AND works:**
- Criteria from both specifications are merged into a single QueryGroup
- RepoDB interprets multiple QueryFields in one QueryGroup as AND logic (default)
- Result: `(IsActive = true) AND (IssueDate >= lastMonth)`
- Sorts from the left specification are preferred; if none, right sorts are used
- SelectFields, Skip, and Take follow the same left-preference rule

---

## Example 8: Chained Composition

```csharp
// Chain multiple AND compositions
var spec = new ActiveInvoicesSpec()
    .And(new RecentInvoicesSpec(DateTime.Now.AddMonths(-1)))
    .And(new HighValueSpec(minAmount: 5000M));

var invoices = connection.Query(spec);
```

---

## Advanced: Building Custom OR Specifications

If you need true OR logic, build a custom specification with database-native expressions:

```csharp
// For OR semantics, use database expressions directly
public sealed class HighValueOrVipSpec : RepoDbSpecification<Invoice>
{
    public HighValueOrVipSpec()
    {
        Where(new QueryGroup(new[]
        {
            new QueryField("(Total > 5000 OR CustomerName = 'VIP')")
        }));
    }
}

var results = connection.Query(new HighValueOrVipSpec());
```

Alternatively, use `Connection.Query()` directly for complex OR scenarios:

```csharp
// Direct query without the specification pattern
var results = connection.Query<Invoice>(
    where: /* your OR criteria */,
    orderBy: /* sorting */
);
```

---

## License

MIT License
