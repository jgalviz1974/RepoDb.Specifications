using RepoDb;
using RepoDb.Enumerations;
using Xunit;

namespace RepoDb.Specifications.Tests;

/// <summary>
/// Unit tests for specification composition (AND and NOT operations).
/// </summary>
public class SpecificationCompositionTests
{
    // Mock specification implementation for testing
    private class MockSpecification<T> : RepoDbSpecification<T>
        where T : class
    {
        public MockSpecification(QueryGroup? criteria = null, IEnumerable<Sort>? sorts = null, int? skip = null, int? take = null, IEnumerable<string>? selectFields = null)
        {
            if (criteria != null)
            {
                this.Where(criteria);
            }

            if (sorts != null)
            {
                foreach (var sort in sorts)
                {
                    this.OrderBy(sort.Field, sort.Direction);
                }
            }

            if (skip.HasValue || take.HasValue)
            {
                this.Page(skip ?? 0, take ?? int.MaxValue);
            }

            if (selectFields != null)
            {
                this.Select(selectFields.ToArray());
            }
        }
    }

    private class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    [Fact]
    public void And_CombinesTwoCriteriaWithAndLogic()
    {
        // Arrange
        var criteria1 = new QueryGroup(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        var criteria2 = new QueryGroup(new[] { new QueryField(nameof(TestEntity.Id), Operation.GreaterThan, 0) });

        var spec1 = new MockSpecification<TestEntity>(criteria: criteria1);
        var spec2 = new MockSpecification<TestEntity>(criteria: criteria2);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined);
        Assert.NotNull(combined.Criteria);
        Assert.IsType<QueryGroup>(combined.Criteria);
    }

    [Fact]
    public void And_WithNullLeftCriteria_UsesRightCriteria()
    {
        // Arrange
        var criteria2 = new QueryGroup(new[] { new QueryField(nameof(TestEntity.Id), Operation.GreaterThan, 0) });

        var spec1 = new MockSpecification<TestEntity>(criteria: null);
        var spec2 = new MockSpecification<TestEntity>(criteria: criteria2);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined.Criteria);
        Assert.Equal(criteria2.QueryFields.Count, combined.Criteria.QueryFields.Count);
    }

    [Fact]
    public void And_WithNullRightCriteria_UsesLeftCriteria()
    {
        // Arrange
        var criteria1 = new QueryGroup(new[] { new QueryField(nameof(TestEntity.IsActive), true) });

        var spec1 = new MockSpecification<TestEntity>(criteria: criteria1);
        var spec2 = new MockSpecification<TestEntity>(criteria: null);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined.Criteria);
        Assert.Equal(criteria1.QueryFields.Count, combined.Criteria.QueryFields.Count);
    }

    [Fact]
    public void And_WithBothNullCriteria_ResultIsNull()
    {
        // Arrange
        var spec1 = new MockSpecification<TestEntity>(criteria: null);
        var spec2 = new MockSpecification<TestEntity>(criteria: null);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.Null(combined.Criteria);
    }

    [Fact]
    public void And_PrefersLeftSorts()
    {
        // Arrange
        var sorts1 = new[] { new Sort(nameof(TestEntity.Name), SortDirection.Asc) };
        var sorts2 = new[] { new Sort(nameof(TestEntity.Id), SortDirection.Desc) };

        var spec1 = new MockSpecification<TestEntity>(sorts: sorts1);
        var spec2 = new MockSpecification<TestEntity>(sorts: sorts2);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.Single(combined.Sorts);
        Assert.Equal(nameof(TestEntity.Name), combined.Sorts[0].Field);
        Assert.Equal(SortDirection.Asc, combined.Sorts[0].Direction);
    }

    [Fact]
    public void And_UsesRightSortsWhenLeftIsEmpty()
    {
        // Arrange
        var sorts2 = new[] { new Sort(nameof(TestEntity.Id), SortDirection.Desc) };

        var spec1 = new MockSpecification<TestEntity>(sorts: null);
        var spec2 = new MockSpecification<TestEntity>(sorts: sorts2);

        // Act
        var combined = spec1.And(spec2);

        // Assert
        Assert.Single(combined.Sorts);
        Assert.Equal(nameof(TestEntity.Id), combined.Sorts[0].Field);
        Assert.Equal(SortDirection.Desc, combined.Sorts[0].Direction);
    }

    [Fact]
    public void Not_CopiesSortsAndFieldsFromInnerSpec()
    {
        // Arrange
        var criteria = new QueryGroup(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        var sorts = new[] { new Sort(nameof(TestEntity.Name), SortDirection.Asc) };

        var spec = new MockSpecification<TestEntity>(criteria: criteria, sorts: sorts);

        // Act
        var negated = spec.Not();

        // Assert
        Assert.Single(negated.Sorts);
        Assert.Equal(nameof(TestEntity.Name), negated.Sorts[0].Field);
    }

    [Fact]
    public void Not_CopiesPagingFromInnerSpec()
    {
        // Arrange
        var criteria = new QueryGroup(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        var spec = new MockSpecification<TestEntity>(criteria: criteria, skip: 10, take: 20);

        // Act
        var negated = spec.Not();

        // Assert
        Assert.Equal(10, negated.Skip);
        Assert.Equal(20, negated.Take);
    }

    [Fact]
    public void And_ThrowsArgumentNullException_WhenOtherIsNull()
    {
        // Arrange
        var spec = new MockSpecification<TestEntity>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => spec.And(null!));
    }

    [Fact]
    public void Not_ThrowsArgumentNullException_WhenSpecIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new NotSpecification<TestEntity>(null!));
    }
}
