using RepoDb.Enumerations;

using System.Data;

namespace RepoDb.Specifications;

/// <summary>
/// Connects the IRepoDbSpecification to RepoDB's Query method via an extension method on IDbConnection.
/// </summary>
public static class DbConnectionExtensions
{
    /// <summary>
    /// Executes a query against the database using the specified specification and returns a collection of entities
    /// that match the criteria.
    /// </summary>
    /// <remarks>This method applies filtering, sorting, field selection, and result limiting as defined by
    /// the provided specification. It leverages RepoDB's query capabilities to construct and execute the query. The
    /// connection is not closed by this method.</remarks>
    /// <typeparam name="TEntity">The type of the entity to query. Must be a reference type.</typeparam>
    /// <param name="connection">The database connection to use for executing the query. Must be open and valid.</param>
    /// <param name="spec">The specification that defines the filtering criteria, sorting, projection, and limit for the query.</param>
    /// <returns>An enumerable collection of entities of type TEntity that satisfy the specification. The collection is empty if
    /// no entities match the criteria.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(
        this IDbConnection connection,
        IRepoDbSpecification<TEntity> spec)
        where TEntity : class
    {
        // ORDER BY
        OrderField[]? orderBy = spec.Sorts.Count > 0
            ? spec.Sorts.Select(s =>
                new OrderField(
                    s.Field,
                    s.Direction == SortDirection.Desc ? Order.Descending : Order.Ascending))
              .ToArray()
            : null;

        // PROJECTION (fields)
        // RepoDB docs show using Field.Parse / Field.From in Query via 'fields:' [1](https://repodb.net/operation/query)
        IEnumerable<Field>? fields = spec.SelectFields.Count > 0
            ? Field.From(spec.SelectFields.ToArray())
            : null;

        // TOP (limit)
        // RepoDB Query supports 'top:' for limiting results. [1](https://repodb.net/operation/query)
        int? top = spec.Take;

        // IMPORTANT:
        // RepoDB Query takes predicates like Expression or QueryField/QueryGroup depending on overload.
        // Your spec.Criteria is QueryGroup, which RepoDB supports for predicates in several ops.
        // We pass it as the first positional argument.
        return connection.Query<TEntity>(
            spec.Criteria,
            orderBy: orderBy,
            top: top,
            fields: fields);
    }

    /// <summary>
    /// Counts the number of entities in the database that match the criteria defined in the specification.
    /// </summary>
    /// <remarks>
    /// This method uses the criteria (WHERE clause) from the specification to count matching entities.
    /// Sorting, field selection, and paging information from the specification are ignored for the count operation.
    /// The connection is not closed by this method.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity to count. Must be a reference type.</typeparam>
    /// <param name="connection">The database connection to use for executing the count. Must be open and valid.</param>
    /// <param name="spec">The specification that defines the filtering criteria for counting.</param>
    /// <returns>The number of entities that match the criteria, or 0 if none match.</returns>
    /// <exception cref="ArgumentNullException">Thrown if connection or spec is null.</exception>
    public static long Count<TEntity>(
        this IDbConnection connection,
        IRepoDbSpecification<TEntity> spec)
        where TEntity : class
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (spec == null)
        {
            throw new ArgumentNullException(nameof(spec));
        }

        return connection.Count<TEntity>(where: spec.Criteria);
    }

    /// <summary>
    /// Determines whether any entity in the database matches the criteria defined in the specification.
    /// </summary>
    /// <remarks>
    /// This method is optimized for checking existence by limiting the query to retrieve only 1 result.
    /// Sorting, field selection, and paging information from the specification are ignored for the existence check.
    /// The connection is not closed by this method.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity to check. Must be a reference type.</typeparam>
    /// <param name="connection">The database connection to use for executing the check. Must be open and valid.</param>
    /// <param name="spec">The specification that defines the filtering criteria for the existence check.</param>
    /// <returns>True if at least one entity matches the criteria; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if connection or spec is null.</exception>
    public static bool Exists<TEntity>(
        this IDbConnection connection,
        IRepoDbSpecification<TEntity> spec)
        where TEntity : class
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (spec == null)
        {
            throw new ArgumentNullException(nameof(spec));
        }

        return connection.Query<TEntity>(
            where: spec.Criteria,
            top: 1).Any();
    }
}