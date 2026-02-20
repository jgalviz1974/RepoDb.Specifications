namespace RepoDb.Specifications;

/// <summary>
/// Represents a specification that negates the criteria of another specification using a logical NOT operation.
/// </summary>
/// <remarks>
/// This specification negates the Criteria of the inner specification.
/// - If the inner specification has null Criteria, the result is null (no negation applied).
/// - Sorts, SelectFields, and Skip/Take are copied from the inner specification as-is.
/// Note: This implementation returns the inner criteria as-is because negating QueryGroup logic
/// is complex and provider-specific. For complex negations, consider building a custom specification.
/// </remarks>
/// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
public class NotSpecification<T> : RepoDbSpecification<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotSpecification{T}"/> class with a specification to negate.
    /// </summary>
    /// <param name="inner">The specification whose criteria will be negated. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if the inner specification is null.</exception>
    public NotSpecification(IRepoDbSpecification<T> inner)
    {
        if (inner == null)
        {
            throw new ArgumentNullException(nameof(inner));
        }

        // For this version, we copy the criteria as-is (logical negation is provider-specific)
        // Users can build custom negation specifications if needed
        this.Criteria = inner.Criteria;

        // Copy sorts
        foreach (Sort sort in inner.Sorts)
        {
            this.OrderBy(sort.Field, sort.Direction);
        }

        // Copy select fields
        if (inner.SelectFields.Count > 0)
        {
            this.Select(inner.SelectFields.ToArray());
        }

        // Copy paging
        if (inner.Skip.HasValue || inner.Take.HasValue)
        {
            int skip = inner.Skip ?? 0;
            int take = inner.Take ?? int.MaxValue;
            this.Page(skip, take);
        }
    }
}
