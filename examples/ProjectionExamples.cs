using RepoDb;
using RepoDb.Enumerations;

namespace Examples;

/// <summary>
/// Projection (SELECT) examples - selecting specific columns.
/// </summary>
public static class ProjectionExamples
{
    /// <summary>
    /// Example 1: Select specific fields for list display.
    /// </summary>
    public class InvoiceListSpec : RepoDbSpecification<Invoice>
    {
        public InvoiceListSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            // Only select fields needed for list display
            Select(
                nameof(Invoice.Id),
                nameof(Invoice.CustomerName),
                nameof(Invoice.Total),
                nameof(Invoice.CreatedDate),
                nameof(Invoice.Status)
            );

            // Sort by date
            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
        }
    }

    /// <summary>
    /// Example 2: Minimal projection for count/summary purposes.
    /// </summary>
    public class InvoiceSummarySpec : RepoDbSpecification<Invoice>
    {
        public InvoiceSummarySpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            // Only essential fields
            Select(
                nameof(Invoice.Id),
                nameof(Invoice.Total)
            );
        }
    }

    /// <summary>
    /// Example 3: Export specification with all fields.
    /// </summary>
    public class InvoiceExportSpec : RepoDbSpecification<Invoice>
    {
        public InvoiceExportSpec(DateTime fromDate, DateTime toDate)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.CreatedDate), Operation.GreaterThanOrEqual, fromDate),
                new QueryField(nameof(Invoice.CreatedDate), Operation.LessThanOrEqual, toDate)
            }));

            // Include all fields for export
            Select(
                nameof(Invoice.Id),
                nameof(Invoice.CustomerName),
                nameof(Invoice.Total),
                nameof(Invoice.CreatedDate),
                nameof(Invoice.DueDate),
                nameof(Invoice.Status),
                nameof(Invoice.IsActive)
            );

            // Sort by ID for export consistency
            OrderBy(nameof(Invoice.Id), SortDirection.Asc);
        }
    }

    /// <summary>
    /// Example 4: Projection for report.
    /// </summary>
    public class InvoiceReportSpec : RepoDbSpecification<Invoice>
    {
        public InvoiceReportSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Invoice.IsActive), true)
            }));

            // Report fields only
            Select(
                nameof(Invoice.Id),
                nameof(Invoice.CustomerName),
                nameof(Invoice.Total),
                nameof(Invoice.CreatedDate)
            );

            OrderBy(nameof(Invoice.CreatedDate), SortDirection.Desc);
            Page(skip: 0, take: 1000); // Limit for reports
        }
    }

    /// <summary>
    /// Example 5: Minimal customer data for dropdown/autocomplete.
    /// </summary>
    public class CustomerDropdownSpec : RepoDbSpecification<Customer>
    {
        public CustomerDropdownSpec()
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Customer.IsActive), true)
            }));

            // Only ID and Name for dropdowns
            Select(
                nameof(Customer.CustomerId),
                nameof(Customer.Name)
            );

            OrderBy(nameof(Customer.Name), SortDirection.Asc);
        }
    }

    /// <summary>
    /// Example 6: API response projection - only necessary fields.
    /// </summary>
    public class ApiOrderResponseSpec : RepoDbSpecification<Order>
    {
        public ApiOrderResponseSpec(int customerId)
        {
            Where(new QueryGroup(new[]
            {
                new QueryField(nameof(Order.CustomerId), customerId)
            }));

            // Only API-safe fields
            Select(
                nameof(Order.OrderId),
                nameof(Order.OrderDate),
                nameof(Order.Amount),
                nameof(Order.Status)
            );

            OrderBy(nameof(Order.OrderDate), SortDirection.Desc);
            Page(skip: 0, take: 50);
        }
    }

    /// <summary>
    /// Usage examples showing how to use projections.
    /// </summary>
    public static void UsageExamples(System.Data.IDbConnection connection)
    {
        // Example 1: Get list for UI
        var listSpec = new InvoiceListSpec();
        var invoices = connection.Query(listSpec).ToList();
        Console.WriteLine($"Retrieved {invoices.Count} invoices for UI display");

        // Example 2: Get summary data
        var summarySpec = new InvoiceSummarySpec();
        var summary = connection.Query(summarySpec).ToList();
        var totalSum = summary.Sum(x => x.Total);
        Console.WriteLine($"Total invoices amount: ${totalSum}");

        // Example 3: Export data
        var exportSpec = new InvoiceExportSpec(
            fromDate: DateTime.Now.AddMonths(-1),
            toDate: DateTime.Now
        );
        var exportData = connection.Query(exportSpec).ToList();
        Console.WriteLine($"Exporting {exportData.Count} invoices");

        // Example 4: Report generation
        var reportSpec = new InvoiceReportSpec();
        var reportData = connection.Query(reportSpec).ToList();
        Console.WriteLine($"Generating report with {reportData.Count} records");

        // Example 5: Dropdown/Autocomplete
        var dropdownSpec = new CustomerDropdownSpec();
        var customers = connection.Query(dropdownSpec).ToList();
        Console.WriteLine($"Loaded {customers.Count} customers for dropdown");

        // Example 6: API Response
        var apiSpec = new ApiOrderResponseSpec(customerId: 123);
        var apiOrders = connection.Query(apiSpec).ToList();
        // Return as JSON: { orders: [...] }
    }

    /// <summary>
    /// Performance benefit example - explain why projection matters.
    /// </summary>
    public static void PerformanceBenefitExample(System.Data.IDbConnection connection)
    {
        // WITHOUT projection - loads ALL columns
        var allColumnsSpec = new RepoDbSpecification<Invoice>();
        allColumnsSpec.Where(new QueryGroup(new[]
        {
            new QueryField(nameof(Invoice.IsActive), true)
        }));
        var allData = connection.Query(allColumnsSpec); // Heavy!

        // WITH projection - loads only what's needed
        var minimalSpec = new InvoiceListSpec(); // Already selects only 5 columns
        var minimalData = connection.Query(minimalSpec); // Lighter!

        // Memory saved, bandwidth saved, faster query
        Console.WriteLine("Projection benefits:");
        Console.WriteLine("- Reduced network traffic");
        Console.WriteLine("- Lower memory footprint");
        Console.WriteLine("- Faster query execution");
        Console.WriteLine("- Cleaner DTO mappings");
    }
}
