using RepoDb.Enumerations;

using Xunit;

namespace RepoDb.Specifications.Tests;

/// <summary>
/// Unit tests for specification composition (AND and NOT operations).
/// </summary>
public class SpecificationCompositionTests
{
    [Fact]
    public void And_CombinesTwoCriteriaWithAndLogic()
    {
        // Arrange
        QueryGroup criteria1 = new(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        QueryGroup criteria2 = new(new[] { new QueryField(nameof(TestEntity.Id), Operation.GreaterThan, 0) });

        MockSpecification<TestEntity> spec1 = new(criteria: criteria1);
        MockSpecification<TestEntity> spec2 = new(criteria: criteria2);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined);
        Assert.NotNull(combined.Criteria);
        _ = Assert.IsType<QueryGroup>(combined.Criteria);
    }

    [Fact]
    public void And_WithNullLeftCriteria_UsesRightCriteria()
    {
        // Arrange
        QueryGroup criteria2 = new(new[] { new QueryField(nameof(TestEntity.Id), Operation.GreaterThan, 0) });

        MockSpecification<TestEntity> spec1 = new(criteria: null);
        MockSpecification<TestEntity> spec2 = new(criteria: criteria2);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined.Criteria);
        Assert.Equal(criteria2.QueryFields.Count, combined.Criteria.QueryFields.Count);
    }

    [Fact]
    public void And_WithNullRightCriteria_UsesLeftCriteria()
    {
        // Arrange
        QueryGroup criteria1 = new(new[] { new QueryField(nameof(TestEntity.IsActive), true) });

        MockSpecification<TestEntity> spec1 = new(criteria: criteria1);
        MockSpecification<TestEntity> spec2 = new(criteria: null);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        Assert.NotNull(combined.Criteria);
        Assert.Equal(criteria1.QueryFields.Count, combined.Criteria.QueryFields.Count);
    }

    [Fact]
    public void And_WithBothNullCriteria_ResultIsNull()
    {
        // Arrange
        MockSpecification<TestEntity> spec1 = new(criteria: null);
        MockSpecification<TestEntity> spec2 = new(criteria: null);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        Assert.Null(combined.Criteria);
    }

    [Fact]
    public void And_PrefersLeftSorts()
    {
        // Arrange
        Sort[] sorts1 = new[] { new Sort(nameof(TestEntity.Name), SortDirection.Asc) };
        Sort[] sorts2 = new[] { new Sort(nameof(TestEntity.Id), SortDirection.Desc) };

        MockSpecification<TestEntity> spec1 = new(sorts: sorts1);
        MockSpecification<TestEntity> spec2 = new(sorts: sorts2);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        _ = Assert.Single(combined.Sorts);
        Assert.Equal(nameof(TestEntity.Name), combined.Sorts[0].Field);
        Assert.Equal(SortDirection.Asc, combined.Sorts[0].Direction);
    }

    [Fact]
    public void And_UsesRightSortsWhenLeftIsEmpty()
    {
        // Arrange
        Sort[] sorts2 = new[] { new Sort(nameof(TestEntity.Id), SortDirection.Desc) };

        MockSpecification<TestEntity> spec1 = new(sorts: null);
        MockSpecification<TestEntity> spec2 = new(sorts: sorts2);

        // Act
        AndSpecification<TestEntity> combined = spec1.And(spec2);

        // Assert
        _ = Assert.Single(combined.Sorts);
        Assert.Equal(nameof(TestEntity.Id), combined.Sorts[0].Field);
        Assert.Equal(SortDirection.Desc, combined.Sorts[0].Direction);
    }

    [Fact]
    public void Not_CopiesSortsAndFieldsFromInnerSpec()
    {
        // Arrange
        QueryGroup criteria = new(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        Sort[] sorts = new[] { new Sort(nameof(TestEntity.Name), SortDirection.Asc) };

        MockSpecification<TestEntity> spec = new(criteria: criteria, sorts: sorts);

        // Act
        NotSpecification<TestEntity> negated = spec.Not();

        // Assert
        _ = Assert.Single(negated.Sorts);
        Assert.Equal(nameof(TestEntity.Name), negated.Sorts[0].Field);
    }

    [Fact]
    public void Not_CopiesPagingFromInnerSpec()
    {
        // Arrange
        QueryGroup criteria = new(new[] { new QueryField(nameof(TestEntity.IsActive), true) });
        MockSpecification<TestEntity> spec = new(criteria: criteria, skip: 10, take: 20);

        // Act
        NotSpecification<TestEntity> negated = spec.Not();

        // Assert
        Assert.Equal(10, negated.Skip);
        Assert.Equal(20, negated.Take);
    }

    [Fact]
    public void And_ThrowsArgumentNullException_WhenOtherIsNull()
    {
        // Arrange
        MockSpecification<TestEntity> spec = new();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => spec.And(null!));
    }

    [Fact]
    public void Not_ThrowsArgumentNullException_WhenSpecIsNull()
    {
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => new NotSpecification<TestEntity>(null!));
    }

    private class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

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
                foreach (Sort sort in sorts)
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
}
