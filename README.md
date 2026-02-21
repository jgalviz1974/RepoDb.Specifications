# RepoDb.Specifications
[![NuGet](https://img.shields.io/nuget/v/jgalviz.RepoDb.Specifications)](https://www.nuget.org/packages/jgalviz.RepoDb.Specifications)
[![GitHub License](https://img.shields.io/github/license/jgalviz1974/RepoDb.Specifications)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/jgalviz.RepoDb.Specifications.svg)](https://www.nuget.org/packages/jgalviz.RepoDb.Specifications)
[![Build Status](https://github.com/jgalviz1974/jgalviz.RepoDb.Specifications/workflows/Build/badge.svg)](https://github.com/jgalviz1974/RepoDb.Specifications/actions)
[![License](https://img.shields.io/github/license/jgalviz1974/RepoDb.Specifications.svg)](LICENSE)

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

---

About me ![](https://user-images.githubusercontent.com/18350557/176309783-0785949b-9127-417c-8b55-ab5a4333674e.gif)My name is José David Galviz Muñoz
===============================================================================================================================================

Entrepreneur, Software Architect, Frontend and Backend Developer and Much More!!!
---------------------------------------------------------------------------------

I started in the world of programming in 1992, since then it has been a journey full of many challenges and emotions.

* 🌍  I'm based in Barranquilla, Colombia.
* ✉️  You can contact me at [jose.david.galviz@gmail.com](mailto:jose.david.galviz@gmail.com)
* 🚀  I'm currently working on [Gasolutions](http://www.gasolutions.com.co/)
* 🤝  I'm open to collaborating on interesting projects open sources and commercials.
* ⚡  If you need me, email me!!!

### Skills


<p align="left">
<a href="https://docs.microsoft.com/en-us/dotnet/csharp/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/csharp-colored.svg" width="36" height="36" alt="C#" /></a><a href="https://git-scm.com/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/git-colored.svg" width="36" height="36" alt="Git" /></a><a href="https://code.visualstudio.com/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/visualstudiocode.svg" width="36" height="36" alt="VS Code" /></a><a href="https://developer.mozilla.org/en-US/docs/Glossary/HTML5" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/html5-colored.svg" width="36" height="36" alt="HTML5" /></a><a href="https://jquery.com/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/jquery-colored.svg" width="36" height="36" alt="JQuery" /></a><a href="https://mui.com/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/materialui-colored.svg" width="36" height="36" alt="Material UI" /></a><a href="https://getbootstrap.com/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/bootstrap-colored.svg" width="36" height="36" alt="Bootstrap" /></a><a href="https://www.adobe.com/uk/products/premiere.html" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/premierepro-colored.svg" width="36" height="36" alt="Premiere Pro" /></a><a href="https://wix.com" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/wix-colored.svg" width="36" height="36" alt="Wix" /></a><a href="https://dotnet.microsoft.com/en-us/" target="_blank" rel="noreferrer"><img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/skills/dot-net-colored.svg" width="36" height="36" alt=".NET" /></a>
</p>


### Socials

<p align="left"> <a href="https://www.dev.to/jgalviz" target="_blank" rel="noreferrer"> <picture> <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/devdotto-dark.svg" /> <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/devdotto.svg" /> <img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/devdotto.svg" width="32" height="32" /> </picture> </a> <a href="https://www.github.com/jgalviz1974" target="_blank" rel="noreferrer"> <picture> <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/github-dark.svg" /> <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/github.svg" /> <img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/github.svg" width="32" height="32" /> </picture> </a> <a href="https://www.linkedin.com/in/jgalviz" target="_blank" rel="noreferrer"> <picture> <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/linkedin-dark.svg" /> <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/linkedin.svg" /> <img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/linkedin.svg" width="32" height="32" /> </picture> </a> <a href="https://medium.com/@jgalviz_1568" target="_blank" rel="noreferrer"> <picture> <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/medium-dark.svg" /> <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/medium.svg" /> <img src="https://raw.githubusercontent.com/danielcranney/readme-generator/main/public/icons/socials/medium.svg" width="32" height="32" /> </picture> </a></p>

### Badges

<b>My GitHub Stats</b>

<a href="http://www.github.com/jgalviz1974"><img src="https://github-readme-streak-stats.herokuapp.com/?user=jgalviz1974&stroke=ffffff&background=1c1917&ring=0891b2&fire=0891b2&currStreakNum=ffffff&currStreakLabel=0891b2&sideNums=ffffff&sideLabels=ffffff&dates=ffffff&hide_border=true" /></a>
