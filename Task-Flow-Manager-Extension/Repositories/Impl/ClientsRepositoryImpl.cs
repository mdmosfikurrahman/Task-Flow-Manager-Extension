using Microsoft.EntityFrameworkCore;
using Task_Flow_Manager_Extension.Data;
using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories.Impl;

public class ClientsRepositoryImpl(TaskFlowManagerExtensionDbContext db) : IClientsRepository
{
    public async Task<List<Client>> FindAllAsync() => await db.Clients.ToListAsync();

    public async Task<Client?> FindByIdAsync(long id) => await db.Clients.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Client> SaveAsync(Client entity)
    {
        if (entity.Id == 0)
            await db.Clients.AddAsync(entity);
        else
            db.Clients.Update(entity);

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await db.Clients.FindAsync(id);
        if (entity != null)
        {
            db.Clients.Remove(entity);
            await db.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByIdAsync(long id) => await db.Clients.AnyAsync(c => c.Id == id);
}