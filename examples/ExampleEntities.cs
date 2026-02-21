using RepoDb;

namespace Examples;

/// <summary>
/// Example entities used in all examples.
/// </summary>
public class Invoice
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? TrackingNumber { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public decimal CreditLimit { get; set; }
}
