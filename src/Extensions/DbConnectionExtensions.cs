using System.Data;
using RepoDb.Enumerations;

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
}