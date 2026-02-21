using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Count and Exists helper method examples.
/// </summary>
public static class CountAndExistsExamples
{
    /// <summary>
    /// Example 1: Count active invoices.
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
    /// Example 2: Count invoices by customer.
    /// </summary>
    public class InvoicesByCustomerSpec : RepoDbSpecification<Invoice>
    {
        public InvoicesByCustomerSpec(string customerName)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.CustomerName), customerName)
            }));
        }
    }

    /// <summary>
    /// Example 3: Check for overdue invoices.
    /// </summary>
    public class OverdueInvoicesSpec : RepoDbSpecification<Invoice>
    {
        public OverdueInvoicesSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.DueDate), Operation.LessThan, DateTime.Now),
                new QueryField(nameof(Invoice.Status), "Pending")
            }));
        }
    }

    /// <summary>
    /// Example 4: Pending orders spec.
    /// </summary>
    public class PendingOrdersSpec : RepoDbSpecification<Order>
    {
        public PendingOrdersSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.Status), "Pending")
            }));
        }
    }

    /// <summary>
    /// Example 5: Low stock products.
    /// </summary>
    public class LowStockProductsSpec : RepoDbSpecification<Product>
    {
        public LowStockProductsSpec(int threshold = 10)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Product.StockQuantity), Operation.LessThanOrEqual, threshold),
                new QueryField(nameof(Product.IsActive), true)
            }));
        }
    }

    /// <summary>
    /// Usage examples for Count.
    /// </summary>
    public static void CountExamples(System.Data.IDbConnection connection)
    {
        // Example 1: Count total active invoices
        var activeSpec = new ActiveInvoicesSpec();
        long activeCount = connection.Count(activeSpec);
        Console.WriteLine($"Total active invoices: {activeCount}");

        // Example 2: Count invoices for specific customer
        var customerSpec = new InvoicesByCustomerSpec("Acme Corp");
        long customerInvoiceCount = connection.Count(customerSpec);
        Console.WriteLine($"Invoices for Acme Corp: {customerInvoiceCount}");

        // Example 3: Count overdue invoices
        var overdueSpec = new OverdueInvoicesSpec();
        long overdueCount = connection.Count(overdueSpec);
        Console.WriteLine($"Overdue invoices: {overdueCount}");

        // Example 4: Dashboard metric - pending orders
        var pendingSpec = new PendingOrdersSpec();
        long pendingOrdersCount = connection.Count(pendingSpec);
        Console.WriteLine($"Pending orders to process: {pendingOrdersCount}");

        // Example 5: Inventory alert - low stock products
        var lowStockSpec = new LowStockProductsSpec(threshold: 20);
        long lowStockCount = connection.Count(lowStockSpec);
        Console.WriteLine($"Products with low stock (< 20): {lowStockCount}");
    }

    /// <summary>
    /// Usage examples for Exists.
    /// </summary>
    public static void ExistsExamples(System.Data.IDbConnection connection)
    {
        // Example 1: Check if there are any overdue invoices
        var overdueSpec = new OverdueInvoicesSpec();
        bool hasOverdueInvoices = connection.Exists(overdueSpec);

        if (hasOverdueInvoices)
        {
            Console.WriteLine("⚠️  WARNING: There are overdue invoices!");
            // Trigger notification or alert
        }
        else
        {
            Console.WriteLine("✓ No overdue invoices");
        }

        // Example 2: Check if customer has any invoices
        var customerSpec = new InvoicesByCustomerSpec("Beta Inc");
        bool customerHasInvoices = connection.Exists(customerSpec);

        if (!customerHasInvoices)
        {
            Console.WriteLine("This is a new customer with no invoice history");
        }

        // Example 3: Check inventory alerts
        var lowStockSpec = new LowStockProductsSpec(threshold: 5);
        bool hasLowStock = connection.Exists(lowStockSpec);

        if (hasLowStock)
        {
            // Trigger inventory reorder process
            Console.WriteLine("🚨 ALERT: Critical low stock items detected!");
        }

        // Example 4: Check for pending actions
        var pendingSpec = new PendingOrdersSpec();
        bool hasPendingOrders = connection.Exists(pendingSpec);

        string status = hasPendingOrders ? "Has pending orders" : "No pending orders";
        Console.WriteLine($"Order processing status: {status}");

        // Example 5: Conditional logic based on existence
        var activeSpec = new ActiveInvoicesSpec();
        if (connection.Exists(activeSpec))
        {
            // Only run expensive report if there are active invoices
            Console.WriteLine("Generating report...");
        }
    }

    /// <summary>
    /// Practical business logic examples.
    /// </summary>
    public static class BusinessLogicExamples
    {
        /// <summary>
        /// Send reminder email if customer has overdue invoices.
        /// </summary>
        public static void SendOverdueReminder(System.Data.IDbConnection connection, string customerName)
        {
            var overdueSpec = new OverdueInvoicesSpec();
            if (connection.Exists(overdueSpec))
            {
                long overdueCount = connection.Count(overdueSpec);
                Console.WriteLine($"Sending overdue payment reminder to {customerName} ({overdueCount} invoices)");
                // Send email here
            }
        }

        /// <summary>
        /// Auto-reorder products if stock is low.
        /// </summary>
        public static void CheckAndReorderProducts(System.Data.IDbConnection connection)
        {
            var lowStockSpec = new LowStockProductsSpec(threshold: 15);

            if (connection.Exists(lowStockSpec))
            {
                long lowStockCount = connection.Count(lowStockSpec);
                Console.WriteLine($"Triggering reorder for {lowStockCount} products");
                // Initiate purchase order
            }
            else
            {
                Console.WriteLine("All products have sufficient stock");
            }
        }

        /// <summary>
        /// Generate daily operations report.
        /// </summary>
        public static void GenerateDailyReport(System.Data.IDbConnection connection)
        {
            var activeInvoicesSpec = new ActiveInvoicesSpec();
            var pendingOrdersSpec = new PendingOrdersSpec();
            var overdueSpec = new OverdueInvoicesSpec();

            long totalInvoices = connection.Count(activeInvoicesSpec);
            long pendingOrders = connection.Count(pendingOrdersSpec);
            long overdueInvoices = connection.Count(overdueSpec);

            Console.WriteLine("=== Daily Operations Report ===");
            Console.WriteLine($"Active Invoices: {totalInvoices}");
            Console.WriteLine($"Pending Orders: {pendingOrders}");
            Console.WriteLine($"Overdue Invoices: {overdueInvoices}");
            Console.WriteLine($"Alert Status: {(overdueInvoices > 0 ? "⚠️  ISSUES" : "✓ OK")}");
        }

        /// <summary>
        /// Dashboard metrics computation.
        /// </summary>
        public static void ComputeDashboardMetrics(System.Data.IDbConnection connection)
        {
            var dashboard = new
            {
                HasPendingOrders = connection.Exists(new PendingOrdersSpec()),
                HasOverdueInvoices = connection.Exists(new OverdueInvoicesSpec()),
                HasLowStock = connection.Exists(new LowStockProductsSpec()),
                PendingOrderCount = connection.Count(new PendingOrdersSpec()),
                OverdueCount = connection.Count(new OverdueInvoicesSpec()),
                LowStockCount = connection.Count(new LowStockProductsSpec())
            };

            Console.WriteLine("Dashboard State:");
            Console.WriteLine($"  Pending Orders: {dashboard.PendingOrderCount} {(dashboard.HasPendingOrders ? "🔴" : "🟢")}");
            Console.WriteLine($"  Overdue Invoices: {dashboard.OverdueCount} {(dashboard.HasOverdueInvoices ? "🔴" : "🟢")}");
            Console.WriteLine($"  Low Stock: {dashboard.LowStockCount} {(dashboard.HasLowStock ? "🔴" : "🟢")}");
        }
    }
}
