namespace Task_Flow_Manager_Extension.Dto.Response;

public class InvoiceResponse
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}