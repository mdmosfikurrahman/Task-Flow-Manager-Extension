namespace Task_Flow_Manager_Extension.Dto.Request;

public class InvoiceRequest
{
    public long ClientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}