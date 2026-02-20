namespace RepoDb.Specifications;

/// <summary>
/// Represents a specification that combines two specifications using a logical AND operation on their criteria.
/// </summary>
/// <remarks>
/// This specification merges the Criteria of the left and right specifications using AND conjunction.
/// - If either specification has null Criteria, the non-null one is used (treating null as "no criteria").
/// - If both have null Criteria, the combined result is null.
/// - When both have criteria, their QueryFields are combined into a single QueryGroup with AND semantics (default behavior in RepoDB).
/// - Sorts: The sorts from the left specification are preferred. If left has no sorts, right sorts are used.
/// - SelectFields: The fields from the left specification are preferred. If left has no fields, right fields are used.
/// - Skip/Take: The paging from the left specification is preferred. If left has no paging, right paging is used.
/// </remarks>
/// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
public class AndSpecification<T> : RepoDbSpecification<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndSpecification{T}"/> class with two specifications to combine.
    /// </summary>
    /// <param name="left">The left specification in the AND operation. Cannot be null.</param>
    /// <param name="right">The right specification in the AND operation. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if either left or right specification is null.</exception>
    public AndSpecification(IRepoDbSpecification<T> left, IRepoDbSpecification<T> right)
    {
        if (left == null)
        {
            throw new ArgumentNullException(nameof(left));
        }

        if (right == null)
        {
            throw new ArgumentNullException(nameof(right));
        }

        // Merge Criteria using AND
        this.Criteria = MergeCriteriaWithAnd(left.Criteria, right.Criteria);

        // Prefer left sorts; if empty, use right sorts
        if (left.Sorts.Count > 0)
        {
            foreach (Sort sort in left.Sorts)
            {
                this.OrderBy(sort.Field, sort.Direction);
            }
        }
        else if (right.Sorts.Count > 0)
        {
            foreach (Sort sort in right.Sorts)
            {
                this.OrderBy(sort.Field, sort.Direction);
            }
        }

        // Prefer left select fields; if empty, use right select fields
        if (left.SelectFields.Count > 0)
        {
            this.Select(left.SelectFields.ToArray());
        }
        else if (right.SelectFields.Count > 0)
        {
            this.Select(right.SelectFields.ToArray());
        }

        // Prefer left paging; if not set, use right paging
        if (left.Skip.HasValue || left.Take.HasValue)
        {
            int skip = left.Skip ?? 0;
            int take = left.Take ?? int.MaxValue;
            this.Page(skip, take);
        }
        else if (right.Skip.HasValue || right.Take.HasValue)
        {
            int skip = right.Skip ?? 0;
            int take = right.Take ?? int.MaxValue;
            this.Page(skip, take);
        }
    }

    /// <summary>
    /// Merges two QueryGroup objects using an AND conjunction.
    /// </summary>
    /// <remarks>
    /// In RepoDB, AND is the default conjunction when multiple QueryFields are combined in a single QueryGroup.
    /// This method extracts QueryFields from both left and right QueryGroups and combines them into a single QueryGroup,
    /// which RepoDB interprets as AND logic.
    /// 
    /// Example:
    /// - Left: QueryField(IsActive, true)
    /// - Right: QueryField(Total, >, 1000)
    /// - Result: QueryGroup([QueryField(IsActive, true), QueryField(Total, >, 1000)]) → IsActive = true AND Total > 1000
    /// </remarks>
    /// <param name="left">The left QueryGroup. Can be null.</param>
    /// <param name="right">The right QueryGroup. Can be null.</param>
    /// <returns>
    /// The merged QueryGroup with AND semantics, or null if both are null.
    /// If one is null, returns the non-null one.
    /// </returns>
    private static QueryGroup? MergeCriteriaWithAnd(QueryGroup? left, QueryGroup? right)
    {
        if (left == null && right == null)
        {
            return null;
        }

        if (left == null)
        {
            return right;
        }

        if (right == null)
        {
            return left;
        }

        // Both are non-null: combine by extracting and merging QueryFields
        // RepoDB: Multiple QueryFields in one QueryGroup = AND logic (default)
        var combinedFields = new List<QueryField>();

        // Extract fields from left QueryGroup
        if (left.QueryFields != null)
        {
            combinedFields.AddRange(left.QueryFields);
        }

        // Extract fields from right QueryGroup
        if (right.QueryFields != null)
        {
            combinedFields.AddRange(right.QueryFields);
        }

        return combinedFields.Count > 0
            ? new QueryGroup(combinedFields)
            : null;
    }
}
