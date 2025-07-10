using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories;

public interface IClientsRepository
{
    Task<List<Client>> FindAllAsync();
    Task<Client?> FindByIdAsync(long id);
    Task<Client> SaveAsync(Client entity);
    Task DeleteByIdAsync(long id);
    Task<bool> ExistsByIdAsync(long id);
}