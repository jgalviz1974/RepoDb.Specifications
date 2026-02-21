using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Sorting and pagination examples.
/// </summary>
public static class SortingAndPaginationExamples
{
    /// <summary>
    /// Example 1: Sort invoices by creation date descending.
    /// </summary>
    public class InvoicesSortedByDateSpec : RepoDbSpecification<Invoice>
    {
        public InvoicesSortedByDateSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
        }
    }

    /// <summary>
    /// Example 2: Sort orders by amount descending.
    /// </summary>
    public class TopOrdersSpec : RepoDbSpecification<Order>
    {
        public TopOrdersSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.Status), "Completed")
            }));

            OrderBy(nameof(Order.Amount), SortDirection.Desc);
        }
    }

    /// <summary>
    /// Example 3: Paginated results with sorting.
    /// </summary>
    public class PaginatedInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public PaginatedInvoicesSpec(int pageNumber, int pageSize)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            // Sort by creation date (newest first)
            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);

            // Calculate skip and take
            int skip = (pageNumber - 1) * pageSize;
            Page(skip, pageSize);
        }
    }

    /// <summary>
    /// Example 4: Multiple sort columns.
    /// </summary>
    public class InvoicesSortedByStatusAndDateSpec : RepoDbSpecification<Invoice>
    {
        public InvoicesSortedByStatusAndDateSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            // Primary sort: by status
            OrderBy(nameof(Invoice.Status), SortDirection.Asc);

            // Secondary sort: by date descending
            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
        }
    }

    /// <summary>
    /// Example 5: Usage - Get invoices page by page.
    /// </summary>
    public static void PaginationExample(System.Data.IDbConnection connection)
    {
        const int pageSize = 20;

        // Get first page
        var page1Spec = new PaginatedInvoicesSpec(pageNumber: 1, pageSize: pageSize);
        var page1 = connection.Query(page1Spec).ToList();

        // Get second page
        var page2Spec = new PaginatedInvoicesSpec(pageNumber: 2, pageSize: pageSize);
        var page2 = connection.Query(page2Spec).ToList();

        // Get specific page
        int requestedPage = 5;
        var pageSpec = new PaginatedInvoicesSpec(pageNumber: requestedPage, pageSize: pageSize);
        var pageData = connection.Query(pageSpec).ToList();

        Console.WriteLine($"Page {requestedPage}: {pageData.Count} invoices");
    }

    /// <summary>
    /// Example 6: Top N results pattern.
    /// </summary>
    public class TopNInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public TopNInvoicesSpec(int topCount)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            OrderBy(nameof(Invoice.Total), SortDirection.Desc);

            // Set page to get top N only
            Page(skip: 0, take: topCount);
        }
    }

    /// <summary>
    /// Example 7: Get top 10 invoices by total amount.
    /// </summary>
    public static void TopNExample(System.Data.IDbConnection connection)
    {
        var topInvoicesSpec = new TopNInvoicesSpec(topCount: 10);
        var topInvoices = connection.Query(topInvoicesSpec).ToList();

        Console.WriteLine($"Top 10 invoices:");
        foreach (var invoice in topInvoices)
        {
            Console.WriteLine($"  ID: {invoice.Id}, Total: ${invoice.Total}");
        }
    }

    /// <summary>
    /// Example 8: Dashboard query - Recent high-value orders.
    /// </summary>
    public class DashboardOrdersSpec : RepoDbSpecification<Order>
    {
        public DashboardOrdersSpec()
        {
            var lastWeek = DateTime.Now.AddDays(-7);

            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.OrderDate), Operation.GreaterThanOrEqual, lastWeek),
                new QueryField(nameof(Order.Amount), Operation.GreaterThan, 500)
            }));

            // Sort by most recent first
            OrderBy(nameof(Order.OrderDate), SortDirection.Desc);

            // Show only top 20 on dashboard
            Page(skip: 0, take: 20);
        }
    }

    public static void DashboardExample(System.Data.IDbConnection connection)
    {
        var dashboardSpec = new DashboardOrdersSpec();
        var dashboardData = connection.Query(dashboardSpec).ToList();

        Console.WriteLine("Dashboard - High Value Orders (Last 7 Days):");
        foreach (var order in dashboardData)
        {
            Console.WriteLine($"  Order #{order.OrderId}: ${order.Amount} on {order.OrderDate:yyyy-MM-dd}");
        }
    }
}
