using System.Data;

using Xunit;

namespace RepoDb.Specifications.Tests;

/// <summary>
/// Unit tests for Count and Exists extension methods.
/// </summary>
public class CountAndExistsTests
{
    [Fact]
    public void Count_ThrowsArgumentNullException_WhenConnectionIsNull()
    {
        // Arrange
        IDbConnection? connection = null;
        SimpleSpecification spec = new();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => connection!.Count(spec));
    }

    [Fact]
    public void Count_ThrowsArgumentNullException_WhenSpecIsNull()
    {
        // Arrange
        IDbConnection? connection = new MockDbConnection();
        IRepoDbSpecification<TestEntity>? spec = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => connection.Count(spec!));
    }

    [Fact]
    public void Exists_ThrowsArgumentNullException_WhenConnectionIsNull()
    {
        // Arrange
        IDbConnection? connection = null;
        SimpleSpecification spec = new();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => connection!.Exists(spec));
    }

    [Fact]
    public void Exists_ThrowsArgumentNullException_WhenSpecIsNull()
    {
        // Arrange
        IDbConnection? connection = new MockDbConnection();
        IRepoDbSpecification<TestEntity>? spec = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => connection.Exists(spec!));
    }

    private class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    private class SimpleSpecification : RepoDbSpecification<TestEntity>
    {
        public SimpleSpecification(QueryGroup? criteria = null)
        {
            if (criteria != null)
            {
                this.Where(criteria);
            }
        }
    }

    /// <summary>
    /// Mock implementation of IDbConnection for testing.
    /// </summary>
    private class MockDbConnection : IDbConnection
    {
        public string? ConnectionString
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State => ConnectionState.Open;

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
    }
}
