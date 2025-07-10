using Microsoft.EntityFrameworkCore;
using Task_Flow_Manager_Extension.Data;
using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Repositories.Impl;

public class InvoicesRepositoryImpl(TaskFlowManagerExtensionDbContext db) : IInvoicesRepository
{
    public async Task<List<Invoices>> FindAllAsync() => await db.Invoices.ToListAsync();

    public async Task<Invoices?> FindByIdAsync(long id) => await db.Invoices.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Invoices> SaveAsync(Invoices entity)
    {
        if (entity.Id == 0)
            await db.Invoices.AddAsync(entity);
        else
            db.Invoices.Update(entity);

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await db.Invoices.FindAsync(id);
        if (entity != null)
        {
            db.Invoices.Remove(entity);
            await db.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByIdAsync(long id) => await db.Invoices.AnyAsync(i => i.Id == id);
}