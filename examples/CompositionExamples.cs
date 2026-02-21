using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Specification composition examples showing how to combine specifications with AND.
/// </summary>
public static class CompositionExamples
{
    /// <summary>
    /// Example 1: Active invoices spec.
    /// </summary>
    public class ActiveInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public ActiveInvoicesSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));
        }
    }

    /// <summary>
    /// Example 2: Recent invoices spec.
    /// </summary>
    public class RecentInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public RecentInvoicesSpec(int daysBack = 30)
        {
            var fromDate = DateTime.Now.AddDays(-daysBack);
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate)
            }));
        }
    }

    /// <summary>
    /// Example 3: High value invoices spec.
    /// </summary>
    public class HighValueInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public HighValueInvoicesSpec(decimal minimumAmount)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.Total), Operation.GreaterThan, minimumAmount)
            }));
        }
    }

    /// <summary>
    /// Example 4: Combining specifications with AND.
    /// Result: Active AND Recent AND HighValue invoices
    /// </summary>
    public static void CombineSpecificationsExample(System.Data.IDbConnection connection)
    {
        var activeSpec = new ActiveInvoicesSpec();
        var recentSpec = new RecentInvoicesSpec(daysBack: 60);
        var highValueSpec = new HighValueInvoicesSpec(minimumAmount: 5000);

        // Combine specifications with AND
        var combinedSpec = activeSpec
            .And(recentSpec)
            .And(highValueSpec);

        // Execute: Get all invoices that are:
        // - Active
        // - Created in the last 60 days
        // - With total > 5000
        var qualifyingInvoices = connection.Query(combinedSpec);
    }

    /// <summary>
    /// Example 5: Two-way composition.
    /// </summary>
    public static void TwoWayCompositionExample(System.Data.IDbConnection connection)
    {
        var activeSpec = new ActiveInvoicesSpec();
        var recentSpec = new RecentInvoicesSpec();

        // Both orders produce the same result
        var combined1 = activeSpec.And(recentSpec);
        var combined2 = recentSpec.And(activeSpec);

        // Same data
        var result1 = connection.Query(combined1);
        var result2 = connection.Query(combined2);
    }

    /// <summary>
    /// Example 6: Practical use case - E-commerce Orders.
    /// </summary>
    public class ActiveOrdersSpec : RepoDbSpecification<Order>
    {
        public ActiveOrdersSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.Status), "Confirmed")
            }));
        }
    }

    public class RecentOrdersSpec : RepoDbSpecification<Order>
    {
        public RecentOrdersSpec(int days)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.OrderDate), Operation.GreaterThanOrEqual, fromDate)
            }));
        }
    }

    public class HighValueOrdersSpec : RepoDbSpecification<Order>
    {
        public HighValueOrdersSpec(decimal minimumAmount)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.Amount), Operation.GreaterThan, minimumAmount)
            }));
        }
    }

    public static void ECommerceOrdersExample(System.Data.IDbConnection connection)
    {
        // Find orders that are:
        // - Confirmed status
        // - Created in last 7 days
        // - With amount > $1000
        var spec = new ActiveOrdersSpec()
            .And(new RecentOrdersSpec(days: 7))
            .And(new HighValueOrdersSpec(minimumAmount: 1000));

        var priorityOrders = connection.Query(spec);

        // Count matching orders
        var count = connection.Count(spec);

        // Check if there are any
        var hasOrders = connection.Exists(spec);
    }
}
