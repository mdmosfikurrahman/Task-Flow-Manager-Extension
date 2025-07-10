using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories;

public interface IInvoicesRepository
{
    Task<List<Invoices>> FindAllAsync();
    Task<Invoices?> FindByIdAsync(long id);
    Task<Invoices> SaveAsync(Invoices entity);
    Task DeleteByIdAsync(long id);
    Task<bool> ExistsByIdAsync(long id);
}