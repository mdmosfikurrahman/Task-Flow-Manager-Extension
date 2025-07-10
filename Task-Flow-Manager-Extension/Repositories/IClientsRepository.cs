using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories;

public interface IClientsRepository
{
    Task<List<Clients>> FindAllAsync();
    Task<Clients?> FindByIdAsync(long id);
    Task<Clients> SaveAsync(Clients entity);
    Task DeleteByIdAsync(long id);
    Task<bool> ExistsByIdAsync(long id);
}