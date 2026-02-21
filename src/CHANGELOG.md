# Changelog - jgalviz.RepoDb.Specifications

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-01-20

### ✨ Added

- **Complete Specification Pattern Implementation**
  - Specification Pattern fully implemented for RepoDB
  - Composition with AND logic
  - Count and Exists helper methods
  - 100% documented with XML comments

- **Core Features**
  - Abstract base class `RepoDbSpecification<T>` for defining reusable query specifications
  - Support for filtering (WHERE), sorting (ORDER BY), paging (SKIP/TAKE), and field projection (SELECT)
  - Interface `IRepoDbSpecification<T>` for specification contracts
  - `AndSpecification<T>` class for combining specifications with AND logic
  - `And()` method on `RepoDbSpecification<T>` for fluent chaining
  - `NotSpecification<T>` class for negating specification criteria
  - `Not()` method for creating negated specifications

- **Database Query Extensions**
  - `Query<T>()` extension method on `IDbConnection` for executing specifications
  - `Count<T>()` extension method for counting entities matching specification criteria
  - `Exists<T>()` extension method for checking entity existence
  - `QueryWithPaging<T>()` extension for advanced paging scenarios

### 🐛 Fixed

- Null-safety handling in specification composition
- Proper QueryField extraction in AND/NOT operations
- Correct parameter naming in extension methods

### 📚 Documentation

- Improved README.md with comprehensive examples
- Architecture guide and best practices
- Usage examples for real-world scenarios
- Contributing guidelines for community contributions
- License documentation

### Known Limitations

- OR logic not supported due to RepoDB 1.13.2-alpha1 API constraints
  - Users can implement custom OR specifications using database-native SQL expressions
  - See README section "Advanced: Building Custom OR Specifications" for workarounds

### Dependencies

- RepoDb 1.13.2-alpha1
- .NET 8.0
- C# 13.0

---

## Installation

```bash
dotnet add package jgalviz.RepoDb.Specifications
```

Or via NuGet Package Manager:
```
Install-Package jgalviz.RepoDb.Specifications
```

**NuGet Package**: https://www.nuget.org/packages/jgalviz.RepoDb.Specifications

---

## Quick Start Example

```csharp
// 1. Define a specification
public sealed class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
{
    public ActiveInvoicesSpec()
    {
        Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true)
        }));
        
        OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
    }
}

// 2. Use it in your repository
var spec = new ActiveInvoicesSpec();

// Query
var invoices = connection.Query(spec);

// Count
long totalCount = connection.Count(spec);

// Check existence
bool hasActive = connection.Exists(spec);
```

---

## Future Releases

### v1.1.0 (Planned)
- [ ] Async query methods (`QueryAsync`, `CountAsync`, `ExistsAsync`)
- [ ] Performance benchmarks and optimization guides
- [ ] Additional real-world scenario examples

### v1.2.0 (Planned)
- [ ] OR support when RepoDB releases stable version with QueryGroup conjunction support
- [ ] Integration examples with popular DDD frameworks
- [ ] Video tutorials

### v2.0.0 (Future)
- [ ] Update to RepoDB stable version (when released)
- [ ] Support for complex projections
- [ ] Complementary packages for advanced scenarios

---

## Support & Contribution

For issues, questions, or contributions, please visit:
- **GitHub Repository**: https://github.com/jgalviz1974/RepoDb.Specifications
- **GitHub Issues**: https://github.com/jgalviz1974/RepoDb.Specifications/issues
- **GitHub Discussions**: https://github.com/jgalviz1974/RepoDb.Specifications/discussions
- **NuGet Package**: https://www.nuget.org/packages/jgalviz.RepoDb.Specifications

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to contribute.

---

## License

This project is licensed under the [MIT License](LICENSE) - see the LICENSE file for details.

Copyright © 2026 José David Galviz Muñoz. All rights reserved.