namespace Task_Flow_Manager_Extension.Models;

public partial class Invoice
{
    public long Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateOnly DateIssued { get; set; }

    public decimal Amount { get; set; }

    public long ClientId { get; set; }

    public string? Notes { get; set; }

    public virtual Client Client { get; set; } = null!;
}