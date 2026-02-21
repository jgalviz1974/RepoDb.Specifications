using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Real-world business scenarios and use cases.
/// </summary>
public static class RealWorldExamples
{
    // ============= E-COMMERCE SCENARIOS =============

    /// <summary>
    /// Scenario 1: Product Search and Filtering
    /// </summary>
    public class ProductSearchSpec : RepoDbSpecification<Product>
    {
        public ProductSearchSpec(string category, decimal maxPrice, bool activeOnly = true)
        {
            var fields = new List<QueryField>
            {
                new QueryField(nameof(Product.Category), category)
            };

            if (maxPrice > 0)
            {
                fields.Add(new QueryField(nameof(Product.Price), Operation.LessThanOrEqual, maxPrice));
            }

            if (activeOnly)
            {
                fields.Add(new QueryField(nameof(Product.IsActive), true));
            }

            Where(new QueryGroup(fields.ToArray()));
            OrderBy(nameof(Product.Name), SortDirection.Asc);
        }
    }

    /// <summary>
    /// Scenario 2: Customer Order History
    /// </summary>
    public class CustomerOrderHistorySpec : RepoDbSpecification<Order>
    {
        public CustomerOrderHistorySpec(int customerId, int pageNumber = 1, int pageSize = 20)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.CustomerId), customerId)
            }));

            OrderBy(nameof(Order.OrderDate), SortDirection.Desc);

            int skip = (pageNumber - 1) * pageSize;
            Page(skip, pageSize);

            // Only show essential fields for history
            Select(
                nameof(Order.OrderId),
                nameof(Order.OrderDate),
                nameof(Order.Amount),
                nameof(Order.Status)
            );
        }
    }

    /// <summary>
    /// Scenario 3: Fulfillment Operations - Orders Ready to Ship
    /// </summary>
    public class OrdersReadyToShipSpec : RepoDbSpecification<Order>
    {
        public OrdersReadyToShipSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.Status), "Confirmed")
            }));

            OrderBy(nameof(Order.OrderDate), SortDirection.Asc);
            Page(skip: 0, take: 100); // Batch size for fulfillment

            Select(
                nameof(Order.OrderId),
                nameof(Order.CustomerId),
                nameof(Order.OrderDate),
                nameof(Order.Amount)
            );
        }
    }

    /// <summary>
    /// Scenario 4: Sales Reporting - Monthly Performance
    /// </summary>
    public class MonthlySalesSpec : RepoDbSpecification<Order>
    {
        public MonthlySalesSpec(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.OrderDate), Operation.GreaterThanOrEqual, startDate),
                new QueryField(nameof(Order.OrderDate), Operation.LessThanOrEqual, endDate),
                new QueryField(nameof(Order.Status), "Completed")
            }));

            OrderBy(nameof(Order.OrderDate), SortDirection.Asc);
        }
    }

    // ============= BILLING/INVOICING SCENARIOS =============

    /// <summary>
    /// Scenario 5: Accounts Receivable - Collect Overdue Payments
    /// </summary>
    public class OverduePaymentsSpec : RepoDbSpecification<Invoice>
    {
        public OverduePaymentsSpec()
        {
            var today = DateTime.Now;

            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.DueDate), Operation.LessThan, today),
                new QueryField(nameof(Invoice.Status), "Pending"),
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            OrderBy(nameof(Invoice.DueDate), SortDirection.Asc);

            Select(
                nameof(Invoice.Id),
                nameof(Invoice.CustomerName),
                nameof(Invoice.Total),
                nameof(Invoice.DueDate)
            );
        }
    }

    /// <summary>
    /// Scenario 6: Revenue Recognition
    /// </summary>
    public class RecognizedRevenueSpec : RepoDbSpecification<Invoice>
    {
        public RecognizedRevenueSpec(DateTime fromDate, DateTime toDate)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate),
                new QueryField(nameof(Invoice.CreatedDate), Operation.LessThanOrEqual, toDate),
                new QueryField(nameof(Invoice.Status), "Completed")
            }));

            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Asc);
        }
    }

    // ============= CUSTOMER MANAGEMENT SCENARIOS =============

    /// <summary>
    /// Scenario 7: High-Value Customer Identification
    /// </summary>
    public class HighValueCustomersSpec : RepoDbSpecification<Customer>
    {
        public HighValueCustomersSpec(decimal creditLimitThreshold = 50000)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Customer.CreditLimit), Operation.GreaterThanOrEqual, creditLimitThreshold),
                new QueryField(nameof(Customer.IsActive), true)
            }));

            OrderBy(nameof(Customer.CreditLimit), SortDirection.Desc);

            Select(
                nameof(Customer.CustomerId),
                nameof(Customer.Name),
                nameof(Customer.Email),
                nameof(Customer.CreditLimit)
            );
        }
    }

    /// <summary>
    /// Scenario 8: New Customers This Month
    /// </summary>
    public class NewCustomersThisMonthSpec : RepoDbSpecification<Customer>
    {
        public NewCustomersThisMonthSpec()
        {
            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Customer.CreatedDate), Operation.GreaterThanOrEqual, monthStart),
                new QueryField(nameof(Customer.CreatedDate), Operation.LessThanOrEqual, monthEnd)
            }));

            OrderBy(nameof(Customer.CreatedDate), SortDirection.Desc);
        }
    }

    // ============= INVENTORY MANAGEMENT =============

    /// <summary>
    /// Scenario 9: Low Stock Alert System
    /// </summary>
    public class LowStockProductsSpec : RepoDbSpecification<Product>
    {
        public LowStockProductsSpec(int criticalThreshold = 10, int warningThreshold = 30)
        {
            // Critical items first
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Product.StockQuantity), Operation.LessThanOrEqual, criticalThreshold),
                new QueryField(nameof(Product.IsActive), true)
            }));

            OrderBy(nameof(Product.StockQuantity), SortDirection.Asc);

            Select(
                nameof(Product.ProductId),
                nameof(Product.Name),
                nameof(Product.Category),
                nameof(Product.StockQuantity)
            );
        }
    }

    // ============= USAGE EXAMPLES =============

    public static class UsageScenarios
    {
        public static void ECommerceExample(System.Data.IDbConnection connection)
        {
            Console.WriteLine("=== E-COMMERCE SCENARIOS ===\n");

            // 1. Search products
            var searchSpec = new ProductSearchSpec(category: "Electronics", maxPrice: 500);
            var products = connection.Query(searchSpec);
            Console.WriteLine($"Found {products.Count()} electronics under $500");

            // 2. Get customer's order history
            var historySpec = new CustomerOrderHistorySpec(customerId: 42, pageNumber: 1, pageSize: 10);
            var orders = connection.Query(historySpec);
            Console.WriteLine($"Retrieved {orders.Count()} orders for customer");

            // 3. Fulfillment batch
            var shippingSpec = new OrdersReadyToShipSpec();
            var toShip = connection.Query(shippingSpec);
            Console.WriteLine($"Ready to ship: {toShip.Count()} orders");

            // 4. Monthly sales report
            var salesSpec = new MonthlySalesSpec(year: 2024, month: 1);
            var monthlySales = connection.Query(salesSpec);
            Console.WriteLine($"January 2024: {monthlySales.Count()} completed orders");
        }

        public static void BillingExample(System.Data.IDbConnection connection)
        {
            Console.WriteLine("\n=== BILLING/INVOICING SCENARIOS ===\n");

            // 1. Overdue payments collection
            var overdueSpec = new OverduePaymentsSpec();
            long overdueCount = connection.Count(overdueSpec);
            Console.WriteLine($"⚠️  {overdueCount} invoices are overdue");

            if (connection.Exists(overdueSpec))
            {
                var overdueInvoices = connection.Query(overdueSpec);
                foreach (var invoice in overdueInvoices)
                {
                    Console.WriteLine($"  - {invoice.CustomerName}: ${invoice.Total} (due {invoice.DueDate:yyyy-MM-dd})");
                }
            }

            // 2. Revenue recognition reporting
            var revenueSpec = new RecognizedRevenueSpec(
                fromDate: new DateTime(2024, 1, 1),
                toDate: new DateTime(2024, 1, 31)
            );
            var recognizedRevenue = connection.Query(revenueSpec);
            Console.WriteLine($"January revenue: {recognizedRevenue.Count()} recognized invoices");
        }

        public static void CustomerManagementExample(System.Data.IDbConnection connection)
        {
            Console.WriteLine("\n=== CUSTOMER MANAGEMENT SCENARIOS ===\n");

            // 1. High-value customer analysis
            var vipSpec = new HighValueCustomersSpec(creditLimitThreshold: 100000);
            var vipCustomers = connection.Query(vipSpec);
            Console.WriteLine($"💎 {vipCustomers.Count()} VIP customers with $100k+ credit");

            // 2. New customer onboarding metrics
            var newCustomerSpec = new NewCustomersThisMonthSpec();
            long newCount = connection.Count(newCustomerSpec);
            Console.WriteLine($"📈 {newCount} new customers this month");
        }

        public static void InventoryExample(System.Data.IDbConnection connection)
        {
            Console.WriteLine("\n=== INVENTORY MANAGEMENT ===\n");

            // 1. Low stock alert
            var lowStockSpec = new LowStockProductsSpec(criticalThreshold: 5);
            bool hasLowStock = connection.Exists(lowStockSpec);

            if (hasLowStock)
            {
                var criticalItems = connection.Query(lowStockSpec);
                Console.WriteLine($"🚨 CRITICAL: {criticalItems.Count()} products critically low");
                foreach (var product in criticalItems)
                {
                    Console.WriteLine($"  - {product.Name}: {product.StockQuantity} units");
                }
            }
            else
            {
                Console.WriteLine("✓ All products have healthy stock levels");
            }
        }
    }
}
