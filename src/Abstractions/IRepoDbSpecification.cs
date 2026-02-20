namespace RepoDb.Specifications;

/// <summary>
/// Defines a specification for querying entities, including criteria, sorting, paging, and field selection options.
/// </summary>
/// <remarks>Implementations of this interface allow for the construction of flexible and reusable query
/// definitions that can be applied to data repositories. Specifications can be combined or reused to encapsulate common
/// filtering, sorting, and paging logic.</remarks>
/// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
public interface IRepoDbSpecification<T>
{
    /// <summary>
    /// Gets the criteria for the specification.
    /// </summary>
    QueryGroup? Criteria { get; }

    /// <summary>
    /// Gets the sorting definitions for the specification.
    /// </summary>
    IReadOnlyList<Sort> Sorts { get; }

    /// <summary>
    /// Gets the paging information for the specification, including the number of records to skip and take. If Skip is null, no paging is applied.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the number of records to take for paging. If Take is null, a default value is used.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the fields to select for the specification. If no fields are specified, all fields are selected.
    /// </summary>
    IReadOnlyList<string> SelectFields { get; }
}