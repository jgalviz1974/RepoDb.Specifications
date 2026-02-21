using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Basic filtering examples showing how to create simple specifications.
/// </summary>
public static class BasicFilteringExamples
{
    /// <summary>
    /// Example 1: Filter active invoices only.
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
    /// Example 2: Filter invoices by status.
    /// </summary>
    public class InvoicesByStatusSpec : RepoDbSpecification<Invoice>
    {
        public InvoicesByStatusSpec(string status)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.Status), status)
            }));
        }
    }

    /// <summary>
    /// Example 3: Filter invoices with total greater than a threshold.
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
    /// Example 4: Filter invoices created in the last N days.
    /// </summary>
    public class RecentInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public RecentInvoicesSpec(int daysBack)
        {
            var fromDate = DateTime.Now.AddDays(-daysBack);
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate)
            }));
        }
    }

    /// <summary>
    /// Example 5: Filter products by category.
    /// </summary>
    public class ProductsByCategorySpec : RepoDbSpecification<Product>
    {
        public ProductsByCategorySpec(string category)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Product.Category), category),
                new QueryField(nameof(Product.IsActive), true)
            }));
        }
    }

    /// <summary>
    /// Usage examples.
    /// </summary>
    public static void UsageExamples(System.Data.IDbConnection connection)
    {
        // Example 1: Get all active invoices
        var activeSpec = new ActiveInvoicesSpec();
        var activeInvoices = connection.Query(activeSpec);

        // Example 2: Get invoices with specific status
        var pendingSpec = new InvoicesByStatusSpec("Pending");
        var pendingInvoices = connection.Query(pendingSpec);

        // Example 3: Get high value invoices
        var highValueSpec = new HighValueInvoicesSpec(1000);
        var highValueInvoices = connection.Query(highValueSpec);

        // Example 4: Get recent invoices
        var recentSpec = new RecentInvoicesSpec(daysBack: 30);
        var recentInvoices = connection.Query(recentSpec);

        // Example 5: Get products in category
        var electronicsSpec = new ProductsByCategorySpec("Electronics");
        var electronics = connection.Query(electronicsSpec);
    }
}
