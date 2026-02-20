namespace RepoDb.Specifications;

/// <summary>
/// Represents a base class for defining specifications that can be used to query entities from a data repository using RepoDB. This class provides methods for setting criteria, sorting, paging, and field selection, allowing for the construction of flexible and reusable query definitions. Derived classes can implement specific query logic by utilizing the provided methods to build up the specification according to their needs.
/// </summary>
/// <typeparam name="T">The type of the entity to which the specification applies. Must be a reference type.</typeparam>
public abstract class RepoDbSpecification<T> : IRepoDbSpecification<T>
    where T : class
{
    private readonly List<Sort> sorts = [];
    private readonly List<string> selectFields = [];

    /// <summary>
    /// Gets or sets the group of query conditions used to filter results.
    /// </summary>
    /// <remarks>Use this property to specify complex filtering logic when constructing queries. The value may
    /// be null if no criteria have been defined.</remarks>
    public QueryGroup? Criteria { get; protected set; }

    /// <summary>
    /// Gets the collection of sort criteria applied to the query.
    /// </summary>
    public IReadOnlyList<Sort> Sorts => this.sorts;

    /// <summary>
    /// Gets or sets the number of items to skip before starting to return results in a paginated query.
    /// </summary>
    /// <remarks>Use this property to implement pagination by specifying how many items to bypass before
    /// retrieving results. If the value is null, no items are skipped.</remarks>
    public int? Skip { get; protected set; }

    /// <summary>
    /// Gets or sets the maximum number of items to return in a paginated query.
    /// </summary>
    /// <remarks>Use this property to limit the number of items returned in a single query. If the value is null, all items are returned.</remarks>
    public int? Take { get; protected set; }

    /// <summary>
    /// Gets the collection of fields to select in the query.
    /// </summary>
    /// <remarks>If no fields are specified, all fields are selected.</remarks>
    public IReadOnlyList<string> SelectFields => this.selectFields;

    /// <summary>
    /// Specifies the filtering criteria to be applied to the query.
    /// </summary>
    /// <param name="criteria">The group of conditions that defines how the query results will be filtered. Cannot be null.</param>
    protected void Where(QueryGroup criteria)
    {
        this.Criteria = criteria;
    }

    /// <summary>
    /// Specifies the field and sort direction to apply to the current sort order.
    /// </summary>
    /// <param name="field">The name of the field to sort by. Cannot be null or empty.</param>
    /// <param name="direction">The direction in which to sort the field. The default is SortDirection.Asc.</param>
    protected void OrderBy(string field, SortDirection direction = SortDirection.Asc)
    {
        this.sorts.Add(new Sort(field, direction));
    }

    /// <summary>
    /// Sets the pagination parameters for skipping a specified number of items and taking a specified number of items
    /// from a collection.
    /// </summary>
    /// <param name="skip">The number of items to skip before starting to take items. Must be greater than or equal to 0.</param>
    /// <param name="take">The maximum number of items to take after skipping. Must be greater than or equal to 0.</param>
    protected void Page(int skip, int take)
    {
        this.Skip = skip;
        this.Take = take;
    }

    /// <summary>
    /// Specifies the fields to include in the selection for the current query.
    /// </summary>
    /// <remarks>Calling this method replaces any previously specified selection fields with the provided
    /// values.</remarks>
    /// <param name="fields">An array of field names to be selected. Each value represents a field to include in the query result. Cannot be
    /// null.</param>
    protected void Select(params string[] fields)
    {
        this.selectFields.Clear();
        this.selectFields.AddRange(fields);
    }

    /// <summary>
    /// Combines this specification with another using a logical AND operation on their criteria.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="AndSpecification{T}"/> that merges the criteria of both specifications.
    /// The criteria from both specifications are combined using AND logic.
    /// If either specification has null criteria, the non-null one is used.
    /// Sorts, SelectFields, and Skip/Take prefer this (left) specification; if not set, the right specification's values are used.
    /// </remarks>
    /// <param name="other">The specification to combine with this one using AND. Cannot be null.</param>
    /// <returns>A new <see cref="AndSpecification{T}"/> representing the combined specifications.</returns>
    /// <exception cref="ArgumentNullException">Thrown if other is null.</exception>
    public AndSpecification<T> And(IRepoDbSpecification<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return new AndSpecification<T>(this, other);
    }

    /// <summary>
    /// Creates a new specification that negates the criteria of this specification using a logical NOT operation.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="NotSpecification{T}"/> that inverts the criteria of this specification.
    /// If this specification has null criteria, the result is also null.
    /// Sorts, SelectFields, and Skip/Take are copied as-is to the new specification.
    /// </remarks>
    /// <returns>A new <see cref="NotSpecification{T}"/> representing the negated specification.</returns>
    public NotSpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}