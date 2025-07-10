using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories;

public interface IInvoicesRepository
{
    Task<List<Invoice>> FindAllAsync();
    Task<Invoice?> FindByIdAsync(long id);
    Task<Invoice> SaveAsync(Invoice entity);
    Task DeleteByIdAsync(long id);
    Task<bool> ExistsByIdAsync(long id);
}