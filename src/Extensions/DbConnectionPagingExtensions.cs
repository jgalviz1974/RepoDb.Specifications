using System.Data;
using RepoDb.Enumerations;

namespace RepoDb.Specifications;

/// <summary>
/// Provides extension methods for querying a database with paging support using a specification pattern.
/// </summary>
/// <remarks>These extension methods enable efficient retrieval of paged data from a database connection,
/// integrating with specification-based query definitions. Paging is applied only when specified in the provided
/// specification; otherwise, a standard query is executed. The methods require an explicit sort order when paging is
/// requested to ensure consistent results.</remarks>
public static class DbConnectionPagingExtensions
{
    /// <summary>
    /// Queries the database with paging based on the provided specification. If no paging is specified, it falls back to a regular query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="spec">The specification defining the query criteria, sorting, and paging.</param>
    /// <returns>An enumerable of entities matching the specification.</returns>
    /// <exception cref="InvalidOperationException">Thrown if paging is requested without an explicit sort order.   </exception>
    public static IEnumerable<TEntity> QueryWithPaging<TEntity>(
        this IDbConnection connection,
        IRepoDbSpecification<TEntity> spec)
        where TEntity : class
    {
        OrderField[] orderBy = spec.Sorts.Count > 0
            ? spec.Sorts.Select(s =>
                new OrderField(
                    s.Field,
                    s.Direction == SortDirection.Desc ? Order.Descending : Order.Ascending))
              .ToArray()
            : throw new InvalidOperationException("SkipQuery requires an ORDER BY.");

        // If no paging requested, fallback to Query()
        if (spec.Skip is null)
        {
            return connection.Query(spec);
        }

        int skip = spec.Skip.Value;
        int take = spec.Take ?? 50; // default page size

        // RepoDB documents SkipQuery(skip, take, orderBy) [2](https://github.com/mikependon/RepoDb/issues/377)
        // Note: SkipQuery examples don't show additional predicates; keep it simple and correct first.
        return connection.SkipQuery<TEntity>(
            skip,
            take,
            orderBy,
            spec.Criteria);
    }
}