namespace RepoDb.Specifications;

/// <summary>
/// Specifies the direction in which to sort a collection.
/// </summary>
/// <remarks>Use this enumeration to indicate whether sorting should be performed in ascending or descending
/// order. This is commonly used in data querying, ordering, or sorting operations.</remarks>
public enum SortDirection
{
    /// <summary>
    /// Specifies ascending order for sorting.
    /// </summary>
    Asc,

    /// <summary>
    /// Specifies descending order for sorting.
    /// </summary>
    Desc,
}

/// <summary>
/// Specifies a field and direction for sorting query results. This record is used to define the sorting criteria in specifications, allowing for flexible and reusable sorting definitions that can be applied to data queries. The Field property indicates the name of the field to sort by, while the Direction property specifies whether the sorting should be in ascending or descending order.
/// </summary>
/// <param name="Field">The name of the field to sort by. Cannot be null or empty.</param>
/// <param name="Direction">The direction in which to sort the field. The default is SortDirection.Asc.</param>
public sealed record Sort(string Field, SortDirection Direction);
