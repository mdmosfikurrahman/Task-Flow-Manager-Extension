namespace Task_Flow_Manager_Extension.Models;

public partial class Client
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? CompanyName { get; set; }

    public virtual ICollection<Invoice> invoices { get; set; } = new List<Invoice>();
}