# Contributing to jgalviz.RepoDb.Specifications

Thank you for your interest in contributing to this project! We welcome contributions from the community. This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
- [Code Style Guidelines](#code-style-guidelines)
- [Testing Guidelines](#testing-guidelines)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)

## Code of Conduct

We are committed to providing a welcoming and inspiring community for all. Please treat all community members with respect and be constructive in your interactions. Any form of harassment or discrimination will not be tolerated.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022+ or Visual Studio Code
- Git

### Setup Development Environment

```bash
# Clone the repository
git clone https://github.com/jgalviz1974/RepoDb.Specifications.git
cd RepoDb.Specifications

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## How to Contribute

### Reporting Bugs

1. **Check existing issues** to avoid duplicates
2. **Create a new issue** with:
   - Clear title describing the problem
   - Detailed description with steps to reproduce
   - Expected vs actual behavior
   - Environment details (OS, .NET version, IDE)
   - Code sample if applicable

### Suggesting Enhancements

1. **Check existing discussions** first
2. **Create a discussion** with your enhancement proposal:
   - What problem does it solve?
   - How would it work?
   - Potential alternatives or trade-offs

### Submitting Code Changes

1. **Fork the repository**

2. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**:
   - Follow the [Code Style Guidelines](#code-style-guidelines)
   - Add/update tests for new functionality
   - Update documentation as needed
   - Keep commits atomic and descriptive

4. **Run tests locally**:
   ```bash
   dotnet test
   dotnet build
   ```

5. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Submit a Pull Request**:
   - Provide clear description of changes
   - Reference related issues
   - Include screenshots/examples if relevant
   - Ensure all checks pass

## Code Style Guidelines

### General Principles

- **Language**: C# 13.0 (.NET 8.0)
- **Null Reference Types**: Enabled (`#nullable enable`)
- **Code Analysis**: StyleCop enforcement enabled
- **Pattern**: Specification Pattern + DDD principles

### Naming Conventions

```csharp
// Classes: PascalCase
public class AndSpecification<T> { }

// Methods: PascalCase
public AndSpecification<T> And(IRepoDbSpecification<T> other) { }

// Parameters: camelCase
public void Method(string paramName) { }

// Local variables: camelCase
var localVariable = value;

// Constants: UPPER_SNAKE_CASE
private const string CONSTANT_VALUE = "value";

// Private fields: _camelCase with underscore
private readonly List<Sort> _sorts = [];
```

### Documentation

All public types and members require XML documentation:

```csharp
/// <summary>
/// Brief description of what this does.
/// </summary>
/// <remarks>
/// Additional context or important notes.
/// </remarks>
/// <typeparam name="T">Description of generic type.</typeparam>
/// <param name="parameter">Description of parameter.</param>
/// <returns>Description of return value.</returns>
/// <exception cref="ArgumentNullException">Thrown when parameter is null.</exception>
public AndSpecification<T> Method<T>(IRepoDbSpecification<T> parameter)
    where T : class
{
}
```

### Formatting

- **Indentation**: 4 spaces (not tabs)
- **Line Length**: Aim for 120 characters, hard limit 150
- **Braces**: Allman style (opening brace on new line)
- **Blank Lines**: Use to separate logical sections

```csharp
public class Example
{
    public void Method()
    {
        if (condition)
        {
            // Code here
        }
    }
}
```

## Testing Guidelines

### Test Structure

```csharp
[Fact]
public void Method_WhenCondition_ExpectedBehavior()
{
    // Arrange
    var input = CreateTestData();
    
    // Act
    var result = SystemUnderTest.Method(input);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(expected, result.Value);
}
```

### Test Coverage

- Aim for >80% code coverage for new features
- Test both happy path and error scenarios
- Use descriptive test names following: `MethodName_Condition_ExpectedBehavior`
- Include edge cases and null scenarios

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverageMetrics=true

# Run specific test class
dotnet test --filter ClassName

# Run with verbose output
dotnet test -v detailed
```

## Commit Guidelines

### Commit Message Format

```
<type>: <subject>

<body>

<footer>
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **test**: Test additions or updates
- **refactor**: Code restructuring
- **chore**: Build, dependencies, etc.

### Examples

```
feat: add async query methods for specifications

Implements QueryAsync, CountAsync, and ExistsAsync extension methods
to support async workflows with specifications.

Closes #42
```

```
fix: handle null criteria correctly in AndSpecification

Previously, null criteria were not handled properly when combining
specifications. Now uses null-coalescing pattern.

Fixes #38
```

```
docs: update README with async examples

Added examples showing how to use new async query methods.
Updated table of contents for clarity.
```

## Pull Request Process

1. **Update related files**:
   - Update `CHANGELOG.md` with your changes
   - Update `README.md` if behavior changes
   - Add/update XML documentation

2. **Ensure quality**:
   - All tests pass locally
   - Code follows style guidelines
   - No compiler warnings
   - Code coverage >80% for new code

3. **Submit PR**:
   - Create pull request with descriptive title
   - Fill PR template (if available)
   - Link related issues
   - Request reviewers

4. **Address feedback**:
   - Respond to review comments promptly
   - Make requested changes
   - Push updates to your branch
   - Re-request review after changes

5. **Merge**:
   - Squash commits before merge (if requested)
   - Use "Squash and merge" for feature branches
   - Use "Create a merge commit" for release branches

## Documentation Updates

When submitting changes that affect usage:

1. **Update README.md** with new examples
2. **Update XML documentation** on all public members
3. **Update CHANGELOG.md** with a new entry under "Unreleased"
4. **Add example** in `/examples` folder if creating new capability

### Documentation Style

- Use clear, concise language
- Include code examples where helpful
- Explain the "why" not just the "how"
- Link to related sections

## Development Workflow

### Create Feature Branch

```bash
git checkout -b feature/add-async-support
```

### Keep Your Branch Updated

```bash
git fetch origin
git rebase origin/main
```

### Clean Up Before PR

```bash
# View commits
git log origin/main..HEAD

# Squash multiple commits if needed
git rebase -i origin/main
```

## Build and Test Locally

```bash
# Full build
dotnet clean
dotnet build -c Release

# Run tests
dotnet test -c Release

# Check code style
dotnet format --verify-no-changes

# Run tests with coverage
dotnet test /p:CollectCoverageMetrics=true
```

## Questions?

- **GitHub Issues**: Ask questions on related issues
- **GitHub Discussions**: Start a discussion for larger topics
- **GitHub Issues**: Open a new issue if it doesn't fit elsewhere

## Recognition

Contributors will be recognized in:
- README.md contributors section (if applicable)
- Release notes for significant contributions
- GitHub contributors page

## License

By contributing, you agree that your contributions will be licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

Thank you for contributing to making jgalviz.RepoDb.Specifications better! 🎉
